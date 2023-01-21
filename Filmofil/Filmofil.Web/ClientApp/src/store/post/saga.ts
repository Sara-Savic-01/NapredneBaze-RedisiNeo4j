import * as saga from "redux-saga/effects";
import { API_URL, POSTS_RESOURCE_URL, USERS_RESOURCE_URL } from "..";
import { apiFetch } from "../../services/auth";
import normalize from "../../services/normalizer";
import { loadCategorieData, loadData } from "../app/action";
import { getCategory, getLoggedUser } from "../app/saga";
import { InitSortPostsAction, openErrorDialog, sortPosts, stopSpinner, InitSortCategoriePostsAction } from "../ui/action";
import { UiActionTypes } from "../ui/types";
import { DislikePostAction, LikePostAction, UserActionTypes } from "../user/types";
import { addCommentToPost, addPost, loadMorePosts, loadMoreCategoriePosts } from "./action";
import { AddCommentToPostAction, FetchCategoriePostsAction, InitAddPostAction, InitLoadMorePostsAction, PostActionTypes, PostState, InitLoadMoreCategoriePostsAction } from "./types";

export function* postSaga() {
  yield saga.all([saga.fork(watchRequests)]);
}

function* watchRequests() {
  yield saga.takeEvery(PostActionTypes.INIT_ADD_POST, initAddPost);
  yield saga.takeEvery(PostActionTypes.INIT_ADD_COMMENT_TO_POST, initAddComment);
  yield saga.takeEvery(UserActionTypes.LIKE_POST, likeDislikeUpdate);
  yield saga.takeEvery(UserActionTypes.DISLIKE_POST, likeDislikeUpdate);
  yield saga.takeEvery(PostActionTypes.FETCH_CATEGORIE_POSTS, loadCategoriePosts);
  yield saga.takeEvery(UiActionTypes.INIT_SORT_POSTS, sort);
  yield saga.takeEvery(PostActionTypes.INIT_LOAD_MORE_POSTS, initLoadPosts);
  yield saga.takeEvery(UiActionTypes.INIT_SORT_CATEGORIE_POSTS, initSortCategoriePosts);
  yield saga.takeEvery(PostActionTypes.INIT_LOAD_MORE_CATEGORIE_POSTS, initLoadMoreCategoriePosts);
}

function* initAddPost(action: InitAddPostAction):any {
  const input = {
    Content: action.input.content,
    CategorieTitle: action.input.categorieTitle,
    AuthorId: action.input.authorId
  }
  const result = yield apiFetch('POST', API_URL + "Post/AddPost", input);
  if(result.success) {
    const post: PostState = {
      ...action.input,
      comments: [],
      categorie: action.input.categorie,
      dislikes: [],
      likes: [],
      likesCount: 0,
      id: result.data,
      timeStamp: ""
    }
    yield saga.put(addPost(post));
  }
  else {
    yield saga.put(openErrorDialog("", "Došlo je do greške, pokušajte ponovo!"));
  }

  yield saga.put(stopSpinner());
}

function* initAddComment(action: AddCommentToPostAction):any {
  const input = {
    Content: action.comment.content,
    PostId: action.comment.postId,
    AuthorId: action.comment.authorId
  }
  const result = yield apiFetch('POST', API_URL + "Post/AddCommentToPost", input);

  if(result.success) {
    yield saga.put(addCommentToPost({...action.comment, id: result.data}));
  }
  else {
    yield saga.put(openErrorDialog("", "Došlo je do greške, pokušajte ponovo!"));
  }

  yield saga.put(stopSpinner());
}

function* likeDislikeUpdate(action: LikePostAction | DislikePostAction):any {
  if(action.type === UserActionTypes.LIKE_POST) {
    return yield apiFetch('POST', API_URL + "Post/LikePost/?postId=" 
    + action.postId + "&userId=" + action.userId + "&categorieTitle=" + action.categorieTitle, "");
  }
  else {
    return yield apiFetch('POST', API_URL + "Post/DislikePost/?postId=" 
    + action.postId + "&userId=" + action.userId + "&categorieTitle=" + action.categorieTitle, "");
  }
}

export function* updateUser(userId: number):any {
  const users = yield saga.select(getUsers);
  const user = users.byId[userId];
  yield apiFetch('PUT', USERS_RESOURCE_URL + userId, user);
}

export function* updatePost(postId: number):any {
  const posts = yield saga.select(getPosts);
  const post = posts.byId[postId];
  yield apiFetch('PUT', POSTS_RESOURCE_URL + postId, post);
}

function* loadCategoriePosts(action: FetchCategoriePostsAction):any {
  const userId = yield saga.select(getLoggedUser);
  const input = {
    Category: action.category,
    PostsIds: [],
    UserId: userId,
    Categorie: action.categorieTitle
  }

  console.log(input);
  const result = yield apiFetch('POST', API_URL + "Post/GetMorePostsOfCategorie", input);

  if(result.success) {
    console.log(result);
    const appData = {
      users: normalize(result.data.users),
      posts: normalize(result.data.posts),
      comments: normalize(result.data.comments),
      categories: normalize(result.data.categories)
    }

    yield saga.put(loadCategorieData(appData));
  }
  else {
    yield saga.put(openErrorDialog("", "Došlo je do greške, pokušajte ponovo!"));
  }

  yield saga.put(stopSpinner());
}

function* sort(action: InitSortPostsAction):any {
  let userId = yield saga.select(getLoggedUser);
  
  let result;
  if(userId === 0) {
    result = yield apiFetch('GET', API_URL + "App/GetClientState/?category=" + action.sortType, "");
  }
  else {
    result = yield apiFetch('GET', API_URL +"App/GetClientState/?category=" + action.sortType + "&userId=" + userId, "");
  }

  if(result.success) {
    yield saga.put(sortPosts(action.sortType));
    const appData = {
      users: normalize(result.data.users),
      posts: normalize(result.data.posts),
      comments: normalize(result.data.comments),
      categories: normalize(result.data.categories)
    }
    yield saga.put(loadData(appData));
  }
  else {
    yield saga.put(openErrorDialog("", "Došlo je do greške, pokušajte ponovo!"));
  }

  yield saga.put(stopSpinner());
}

function* initLoadPosts(action: InitLoadMorePostsAction):any {
  const userId = yield saga.select(getLoggedUser);
  const category = yield saga.select(getCategory); 
  
  const input = {
    Category: category,
    PostsIds: action.posts,
    UserId: userId,
    Categorie: ""
  }

  const result = yield apiFetch('POST', API_URL + "Post/GetMorePosts", input);

  if(result.success) {
    console.log(result);
    const appData = {
      users: normalize(result.data.users),
      posts: normalize(result.data.posts),
      comments: normalize(result.data.comments),
      categories: normalize(result.data.categories)
    }
    
    yield saga.put(loadMorePosts(appData));
  }
  else {
    window.location.reload();
  }

  yield saga.put(stopSpinner());
}

function* initSortCategoriePosts(action: InitSortCategoriePostsAction):any {
  let userId = yield saga.select(getLoggedUser);

  const input = {
    Category: action.sortType,
    PostsIds: [],
    UserId: userId,
    Categorie: action.categorie
  }
  console.log(action);
  const result = yield apiFetch('POST', API_URL + "Post/GetMorePostsOfCategorie", input);

  if(result.success) {
    yield saga.put(sortPosts(action.sortType));
    const appData = {
      users: normalize(result.data.users),
      posts: normalize(result.data.posts),
      comments: normalize(result.data.comments),
      categories: normalize(result.data.categories)
    }
    yield saga.put(loadCategorieData(appData));
  }
  else {
    yield saga.put(openErrorDialog("", "Došlo je do greške, pokušajte ponovo!"));
  }

  yield saga.put(stopSpinner());
}

function* initLoadMoreCategoriePosts(action: InitLoadMoreCategoriePostsAction):any {
  const userId = yield saga.select(getLoggedUser);

  const input = {
    Category: action.category,
    PostsIds: action.posts,
    UserId: userId,
    Categorie: action.categorie
  }
  console.log(input);
  const result = yield apiFetch('POST', API_URL + "Post/GetMorePostsOfCategorie", input);

  if(result.success) {
    console.log(result);
    const appData = {
      users: normalize(result.data.users),
      posts: normalize(result.data.posts),
      comments: normalize(result.data.comments),
      categories: normalize(result.data.categories)
    }
    
    yield saga.put(loadMoreCategoriePosts(appData));
  }
  else {
    yield saga.put(openErrorDialog("", "Došlo je do greške, pokušajte ponovo!"));
  }

  yield saga.put(stopSpinner());
}

export const getPosts = (state: any) => state.posts;
export const getUsers = (state: any) => state.users;