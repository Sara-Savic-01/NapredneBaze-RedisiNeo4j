import { Action } from "redux";
import { NormalizedObjects } from "..";
import { CommentState } from "../comment/types";
import { AppState } from "../app/types";

export interface PostState {
  id: number
  authorId: number,
  content: string,
  likes: number[],
  dislikes: number[],
  likesCount: number,
  comments: number[]
  categorie: number,
  categorieTitle: string,
  timeStamp: string
}

export interface InitAddPostInput {
  content: string,
  categorie: number,
  categorieTitle: string,
  authorId: number
}

export enum PostActionTypes {
  LOAD_POSTS = "post/LOAD_POSTS",
  LOAD_POSTS_SUCCESS = "post/LOAD_POSTS_SUCCESS",
  ADD_POST = "post/ADD_POST",
  INIT_ADD_POST = "post/INIT_ADD_POST",
  ADD_COMMENT_TO_POST = "post/ADD_COMMENT_TO_POST",
  INIT_ADD_COMMENT_TO_POST = "post/INIT_ADD_COMMENT_TO_POST",
  FETCH_CATEGORIE_POSTS = "post/FETCH_CATEGORIE_POSTS",
  LOAD_MORE_POSTS = "post/LOAD_MORE_POSTS",
  INIT_LOAD_MORE_POSTS = "post/INIT_LOAD_MORE_POSTS",
  LOAD_MORE_CATEGORIE_POSTS = "post/LOAD_MORE_CATEGORIE_POSTS",
  INIT_LOAD_MORE_CATEGORIE_POSTS = "post/INIT_LOAD_MORE_CATEGORIE_POSTS"
}

export interface AddPostAction extends Action {
  post: PostState
}

export interface InitAddPostAction extends Action {
  input: InitAddPostInput
}

export interface LoadPostsSuccessAction extends Action {
  posts: NormalizedObjects<PostState>
}

export interface LoadPostsAction extends Action {
  posts: NormalizedObjects<PostState>
}

export interface AddCommentToPostAction extends Action {
  comment: CommentState
}

export interface InitAddCommentToPostAction extends Action {
  comment: CommentState
}

export interface FetchCategoriePostsAction extends Action {
  categorieTitle: string,
  category: string
}

export interface LoadMorePostsAction extends Action {
  appState: AppState
}

export interface InitLoadMorePostsAction extends Action {
  posts: number[]
}

export interface InitLoadMoreCategoriePostsAction extends Action {
  posts: number[], category: string, categorie: string
}