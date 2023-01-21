import { Action } from "redux";
import { NormalizedObjects } from "..";
import { AppState } from "../app/types";

export interface UserState {
  id: number,
  username: string,
  email: string,
  password: string,
  posts: number[],
  comments: number[],
  likedPosts: number[],
  dislikedPosts: number[],
  likedComments: number[],
  dislikedComments: number[],
  categories: number[],
  messages: number[]
}

export interface Error {
  error: boolean,
  errorText: string
}

export interface InitSignUpData {
  username: string,
  email: string,
  password: string
}

export interface SignUpData {
  username: string,
  email: string,
  password: string,
  id: number
}

export enum UserActionTypes {
  LOAD_USERS = "user/LOAD_USERS",
  LOAD_USERS_SUCCESS = "user/LOAD_USERS_SUCCESS",
  SIGN_UP = "user/SIGN_UP",
  INIT_SIGN_UP = "user/INIT_SIGN_UP",
  LIKE_POST = "user/LIKE_POST",
  DISLIKE_POST = "user/DISLIKE_POST",
  LIKE_COMMENT = "user/LIKE_COMMENT",
  DISLIKE_COMMENT = "user/DISLIKE_COMMENT",
  LOGIN = "user/LOGIN",
  LOAD_USER_DATA = "user/LOAD_USER_DATA",
  FETCH_USER_DATA = "user/FETCH_USER_DATA"
}

export interface SignUpAction extends Action {
  signUpData: SignUpData
}

export interface InitSignUpAction extends Action {
  signUpData: InitSignUpData
}

export interface LoadUsersAction extends Action {
  users: NormalizedObjects<UserState>
}

export interface LoadUsersSuccessAction extends Action {
  users: NormalizedObjects<UserState>
}

export interface LikePostAction extends Action {
  userId: number,
  postId: number,
  categorieTitle: string,
}

export interface LikeCommentAction extends Action {
  userId: number,
  commentId: number,
  categorieTitle: string,
}

export interface DislikePostAction extends Action {
  userId: number,
  postId: number,
  categorieTitle: string,
}

export interface DislikeCommentAction extends Action {
  userId: number,
  commentId: number,
  categorieTitle: string,
}

export interface LoginAction extends Action {
  username: string, password: string
}

export interface LoadUserDataAction extends Action {
  appState: AppState
}