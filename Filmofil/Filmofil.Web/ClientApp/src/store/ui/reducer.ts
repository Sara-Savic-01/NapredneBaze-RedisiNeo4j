import { Reducer } from "redux";
import { getItemFromLocalStorage, LOGGED_USER_KEY } from "../../services/local-storage";
import { AppActionTypes, LoadCategorieDataAction, LoadDataAction } from "../app/types";
import { LoadMorePostsAction, LoadPostsAction, PostActionTypes } from "../post/types";
import { LoadUserDataAction, UserActionTypes } from "../user/types";
import { OpenErrorDialogAction, SetLoggedUserAction, SortPostsAction, SelectChatCategorieAction } from "./action";
import { UiActionTypes, UiState } from "./types";

const initialState: UiState = {
  isOpenedSinglePost: false,
  loggedUser: 0,
  openedPostId: 0,
  homePosts: [],
  categoriePosts: [],
  userPosts: [],
  isLoginDialogOpened: false,
  isSignupDialogOpened: false,
  postsSortType: "popular",
  error: false,
  errorMessage: "",
  errorTitle: "",
  spinner: false,
  selectedChatCategorie: 0,
  hubConnectionId: ""
}

const reducer: Reducer<UiState> = (state = initialState, action) => {
  switch (action.type) {
    case AppActionTypes.FETCH_DATA: {
      const loggedUser: number | null = getItemFromLocalStorage<number>(LOGGED_USER_KEY);
      if(loggedUser) return { ...state, loggedUser }
      else return state;
    }
    case UiActionTypes.SET_LOGGED_USER: {
      return {
        ...state, 
        loggedUser: (action as SetLoggedUserAction).user,
        homePosts: [],
        categoriePosts: [],
        userPosts: []
      }
    }
    case UiActionTypes.LOGOUT_USER: {
      return {
        ...state,
        loggedUser: 0,
        homePosts: [],
        categoriePosts: [],
        userPosts: []
      }
    }
    case UiActionTypes.OPEN_LOGIN_DIALOG: {
      return {...state, isLoginDialogOpened: true}
    }
    case UiActionTypes.CLOSE_LOGIN_DIALOG: {
      return {...state, isLoginDialogOpened: false}
    }
    case UiActionTypes.OPEN_SIGNUP_DIALOG: {
      return {...state, isSignupDialogOpened: true}
    }
    case UiActionTypes.CLOSE_SIGNUP_DIALOG: {
      return {...state, isSignupDialogOpened: false}
    }
    case UiActionTypes.INIT_SORT_POSTS: {
      return {
        ...state, 
        postsSortType: (action as SortPostsAction).sortType,
        homePosts: []
      }
    }
    case UiActionTypes.INIT_SORT_CATEGORIE_POSTS: {
      return {
        ...state,
        categoriePosts: []
      }
    }
    case PostActionTypes.LOAD_POSTS: {
      return {
        ...state, homePosts: (action as LoadPostsAction).posts.allIds
      }
    }
    case UiActionTypes.OPEN_ERROR_DIALOG: {
      const {message, title} = (action as OpenErrorDialogAction);
      return {
        ...state,
        error: true,
        errorMessage: message,
        errorTitle: title
      }
    }
    case UiActionTypes.CLOSE_ERROR_DIALOG: {
      return {
        ...state,
        error: false,
        errorMessage: "",
        errorTitle: ""
      }
    }
    case UiActionTypes.START_SPINNER: {
      return {
        ...state,
        spinner: true
      }
    }
    case UiActionTypes.STOP_SPINNER: {
      return {
        ...state,
        spinner: false
      }
    }
    case AppActionTypes.LOAD_DATA: {
      return {
        ...state, 
        homePosts: [...state.homePosts, ...(action as LoadDataAction).appState.posts.allIds],
        userPosts: [],
        categoriePosts: []
      }
    }
    case AppActionTypes.LOAD_CATEGORIE_DATA: {
      return {
        ...state, categoriePosts: (action as LoadCategorieDataAction).appState.posts.allIds
      }
    }
    case UserActionTypes.LOAD_USER_DATA: {
      return {
        ...state, userPosts: (action as LoadUserDataAction).appState.posts.allIds
      }
    }
    case PostActionTypes.LOAD_MORE_CATEGORIE_POSTS: {
      return {
        ...state, categoriePosts: [...state.categoriePosts, ...(action as LoadMorePostsAction).appState.posts.allIds]
      }
    }
    case PostActionTypes.LOAD_MORE_POSTS: {
      return {
        ...state, homePosts: [...state.homePosts, ...(action as LoadMorePostsAction).appState.posts.allIds]
      }
    }
    case UiActionTypes.SELECT_CHAT_CATEGORIE: {
      return {
        ...state,
        selectedChatCategorie: (action as SelectChatCategorieAction).categorie
      }
    }
    default: return state;
  }
}

export { reducer as uiReducer };

