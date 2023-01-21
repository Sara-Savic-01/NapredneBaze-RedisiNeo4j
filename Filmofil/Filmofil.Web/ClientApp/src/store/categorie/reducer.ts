import { Reducer } from 'redux';
import { AppActionTypes, LoadDataAction } from '../app/types';
import { AddPostAction, PostActionTypes } from '../post/types';
import { NormalizedObjects } from './../index';
import { CategorieActionTypes, CategorieState, JoinCategorieAction, LeaveCategorieAction } from "./types";
import { MessageActionTypes, SendMessageAction } from '../message/types';

const initialState: NormalizedObjects<CategorieState> = {
  allIds: [],
  byId: {},
  isLoaded: false
}

const reducer: Reducer<NormalizedObjects<CategorieState>> = (state = initialState, action) => {
  switch(action.type) {
    case AppActionTypes.FETCH_DATA: { return state; }
    case AppActionTypes.LOAD_DATA: {
      return {
        ...state,
        ...(action as LoadDataAction).appState.categories,
        isLoaded: true
      }
    }
    case CategorieActionTypes.LEAVE_CATEGORIE: {
      const {categorie, user} = (action as LeaveCategorieAction);
      let users: number[] = state.byId[categorie].users.filter(id => id !== user);
      return {
        ...state, 
        byId: {
          ...state.byId,
          [categorie]: {
            ...state.byId[categorie],
            users
          }
        }
      }
    }
    case CategorieActionTypes.JOIN_CATEGORIE: {
      const {categorie, user} = (action as JoinCategorieAction);
      return {
        ...state,
        byId: {
          ...state.byId,
          [categorie]: {
            ...state.byId[categorie],
            users: [...state.byId[categorie].users, user]
          }
        }
      }
    }
    case PostActionTypes.ADD_POST: {
      const { post } = (action as AddPostAction);
      return {
        ...state,
        byId: {
          ...state.byId,
          [post.categorie]: {
            ...state.byId[post.categorie],
            posts: [...state.byId[post.categorie].posts, post.id]
          }
        }
      }
    }
    case MessageActionTypes.SEND_MESSAGE: {
      const { message } = (action as SendMessageAction);
      return {
        ...state,
        byId: {
          ...state.byId,
          [message.categorie]: {
            ...state.byId[message.categorie],
            messages: [...state.byId[message.categorie].messages, message.id]
          }
        }
      }
    }
    default: return state;
  }
}

export { reducer as categorieReducer };
