import { UiActionTypes } from "./types";
import { Action } from "redux";

export interface SetLoggedUserAction extends Action {
  user: number
}

export function setLoggedUser(user: number): SetLoggedUserAction {
  return {type: UiActionTypes.SET_LOGGED_USER, user }
}

export interface SetTopicsAction extends Action {
  topics: number[]
}

export function setTopics(topics: number[]): SetTopicsAction {
  return {type: UiActionTypes.SET_TOPICS, topics }
}

export interface SortPostsAction extends Action {
  sortType: string
}

export interface InitSortPostsAction extends Action {
  sortType: string
}

export function sortPosts(sortType: string) : SortPostsAction {
  return { type: UiActionTypes.SORT_POSTS, sortType }
}

export function initSortPosts(sortType: string) : InitSortPostsAction {
  return { type: UiActionTypes.INIT_SORT_POSTS, sortType }
}

export interface InitSortCategoriePostsAction extends Action {
  sortType: string, categorie: string
}

export function initSortCategoriePosts(sortType: string, categorie: string) : InitSortCategoriePostsAction {
  return { type: UiActionTypes.INIT_SORT_CATEGORIE_POSTS, sortType, categorie }
}

export interface OpenCategorieAction extends Action {
  posts: number[]
}

export function openCategorie(posts: number[]) : OpenCategorieAction {
  return { type: UiActionTypes.OPEN_CATEGORIE, posts: posts }
}

export interface OpenErrorDialogAction extends Action {
  title: string, message: string
}

export function openErrorDialog(title: string, message: string): OpenErrorDialogAction {
  return { type: UiActionTypes.OPEN_ERROR_DIALOG, title, message }
}

export interface SetPostsAction extends Action {
  posts: number[]
}

export function setHomePosts(posts: number[]): SetPostsAction {
  return { type: UiActionTypes.SET_HOME_POSTS, posts }
}

export function setCategoriePosts(posts: number[]): SetPostsAction {
  return { type: UiActionTypes.SET_CATEGORIE_POSTS, posts }
}

export function setUserPosts(posts: number[]): SetPostsAction {
  return { type: UiActionTypes.SET_USER_POSTS, posts }
}

export interface SelectChatCategorieAction extends Action {
  categorie: number
}

export function selectChatCategorie(categorie: number): SelectChatCategorieAction {
  return { type: UiActionTypes.SELECT_CHAT_CATEGORIE, categorie }
}

export function logoutUser() { return {type: UiActionTypes.LOGOUT_USER }}
export function openLoginDialog() { return {type: UiActionTypes.OPEN_LOGIN_DIALOG }}
export function closeLoginDialog() { return {type: UiActionTypes.CLOSE_LOGIN_DIALOG }}
export function openSignupDialog() { return {type: UiActionTypes.OPEN_SIGNUP_DIALOG }}
export function closeSignupDialog() { return {type: UiActionTypes.CLOSE_SIGNUP_DIALOG }}
export function closeErrorDialog() { return {type: UiActionTypes.CLOSE_ERROR_DIALOG} }
export function startSpinner() { return {type: UiActionTypes.START_SPINNER} }
export function stopSpinner() { return {type: UiActionTypes.STOP_SPINNER} }