import { NormalizedObjects } from "..";
import { CommentState } from "../comment/types";
import { AddCommentToPostAction, AddPostAction, FetchCategoriePostsAction, InitAddCommentToPostAction, InitAddPostAction, InitAddPostInput, LoadPostsAction, LoadPostsSuccessAction, PostActionTypes, PostState, LoadMorePostsAction, InitLoadMorePostsAction, InitLoadMoreCategoriePostsAction } from "./types";
import { AppState } from "../app/types";

export function addPost(post: PostState): AddPostAction { 
  return {type: PostActionTypes.ADD_POST, post }
}

export function initAddPost(input: InitAddPostInput): InitAddPostAction {
  return { type: PostActionTypes.INIT_ADD_POST, input }
}

export function loadPosts(posts: NormalizedObjects<PostState>): LoadPostsAction {
  return { type: PostActionTypes.LOAD_POSTS, posts};
}

export function loadPostsSuccess(posts: NormalizedObjects<PostState>): LoadPostsSuccessAction {
  return { type: PostActionTypes.LOAD_POSTS_SUCCESS, posts};
}

export function addCommentToPost(comment: CommentState): AddCommentToPostAction {
  return {type: PostActionTypes.ADD_COMMENT_TO_POST, comment }
}

export function initAddCommentToPost(comment: CommentState): InitAddCommentToPostAction {
  return {type: PostActionTypes.INIT_ADD_COMMENT_TO_POST, comment }
}

export function fetchCategoriePosts(categorieTitle: string, category: string): FetchCategoriePostsAction {
  return { 
    type: PostActionTypes.FETCH_CATEGORIE_POSTS, 
    categorieTitle: categorieTitle,
    category: category
  }
}

export function initLoadMorePosts(posts: number[]): InitLoadMorePostsAction {
  return { type: PostActionTypes.INIT_LOAD_MORE_POSTS, posts }
}

export function loadMorePosts(appState: AppState): LoadMorePostsAction {
  return { type: PostActionTypes.LOAD_MORE_POSTS, appState }
}

export function initLoadMoreCategoriePosts(posts: number[], category: string, categorie: string): InitLoadMoreCategoriePostsAction {
  return { type: PostActionTypes.INIT_LOAD_MORE_CATEGORIE_POSTS, posts, category, categorie }
}

export function loadMoreCategoriePosts(appState: AppState): LoadMorePostsAction {
  return { type: PostActionTypes.LOAD_MORE_CATEGORIE_POSTS, appState }
}