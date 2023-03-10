export interface UiState {
  homePosts: number[],
  categoriePosts: number[],
  userPosts: number[],
  openedPostId: number,
  isOpenedSinglePost: boolean,
  loggedUser: number,
  isLoginDialogOpened: boolean,
  isSignupDialogOpened: boolean,
  postsSortType: string,
  error: boolean,
  errorMessage: string,
  errorTitle: string,
  spinner: boolean,
  selectedChatCategorie: number,
  hubConnectionId: string
}

export enum UiActionTypes {
  SET_HOME_POSTS = "ui/SET_HOME_POSTS",
  SET_CATEGORIE_POSTS = "ui/SET_CATEGORIE_POSTS",
  SET_USER_POSTS = "ui/SET_USER_POSTS",
  SET_LOGGED_USER = "ui/SET_LOGGED_USER",
  SET_TOPICS = "ui/SET_TOPICS",
  LOGOUT_USER = "ui/LOGOUT_USER",
  OPEN_LOGIN_DIALOG = "ui/OPEN_LOGIN_DIALOG",
  CLOSE_LOGIN_DIALOG = "ui/CLOSE_LOGIN_DIALOG",
  OPEN_SIGNUP_DIALOG = "ui/OPEN_SIGNUP_DIALOG",
  CLOSE_SIGNUP_DIALOG = "ui/CLOSE_SIGNUP_DIALOG",
  SORT_POSTS = "ui/SORT_POSTS",
  INIT_SORT_POSTS = "ui/INIT_SORT_POSTS",
  INIT_SORT_CATEGORIE_POSTS = "ui/INIT_SORT_CATEGORIE_POSTS",
  OPEN_CATEGORIE = "ui/OPEN_CATEGORIE",
  OPEN_ERROR_DIALOG = "ui/OPEN_ERROR_DIALOG",
  CLOSE_ERROR_DIALOG = "ui/CLOSE_ERROR_DIALOG",
  START_SPINNER = "ui/START_SPINNER",
  STOP_SPINNER = "ui/STOP_SPINNER",
  CREATE_HUB_CONNECTION = "ui/CREATE_HUB_CONNECTION",
  SELECT_CHAT_CATEGORIE = "ui/SELECT_CHAT_CATEGORIE"
}