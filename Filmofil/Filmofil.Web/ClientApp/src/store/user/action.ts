import { NormalizedObjects } from "..";
import { DislikeCommentAction, DislikePostAction, InitSignUpAction, InitSignUpData, LikeCommentAction, LikePostAction, LoadUsersAction, LoadUsersSuccessAction, LoginAction, SignUpAction, SignUpData, UserActionTypes, UserState, LoadUserDataAction } from "./types";
import { AppState } from "../app/types";

export function signUp(signUpData: SignUpData): SignUpAction {
  return { type: UserActionTypes.SIGN_UP, signUpData };
}

export function initSignUp(signUpData: InitSignUpData): InitSignUpAction {
  return { type: UserActionTypes.INIT_SIGN_UP, signUpData };
}

export function loadUsers(users: NormalizedObjects<UserState>): LoadUsersAction {
  return { type: UserActionTypes.LOAD_USERS, users };
}

export function loadUsersSuccess(users: NormalizedObjects<UserState>): LoadUsersSuccessAction {
  return { type: UserActionTypes.LOAD_USERS_SUCCESS, users };
}

export function likePost(likePostInput: { userId: number, postId: number, categorieTitle: string }): LikePostAction {
  return {
    type: UserActionTypes.LIKE_POST,
    postId: likePostInput.postId,
    userId: likePostInput.userId,
    categorieTitle: likePostInput.categorieTitle
  };
}

export function likeComment(likeCommentInput: { userId: number, commentId: number, categorieTitle: string }): LikeCommentAction {
  return { 
    type: UserActionTypes.LIKE_COMMENT,
    commentId: likeCommentInput.commentId,
    userId: likeCommentInput.userId,
    categorieTitle: likeCommentInput.categorieTitle
  };
}

export function dislikePost(dislikePostInput: { userId: number, postId: number, categorieTitle: string }): DislikePostAction {
  return {
    type: UserActionTypes.DISLIKE_POST,
    postId: dislikePostInput.postId,
    userId: dislikePostInput.userId,
    categorieTitle: dislikePostInput.categorieTitle
  };
}

export function dislikeComment(dislikeCommentInput: { userId: number, commentId: number, categorieTitle: string }): DislikeCommentAction {
  return { 
    type: UserActionTypes.DISLIKE_COMMENT,
    commentId: dislikeCommentInput.commentId,
    userId: dislikeCommentInput.userId,
    categorieTitle: dislikeCommentInput.categorieTitle
  };
}

export function login(username: string, password: string): LoginAction {
  return { type: UserActionTypes.LOGIN, username, password }
}

export function loadUserData(appState: AppState): LoadUserDataAction {
  return { type: UserActionTypes.LOAD_USER_DATA, appState }
}

export function fetchUserData() { return { type: UserActionTypes.FETCH_USER_DATA } };