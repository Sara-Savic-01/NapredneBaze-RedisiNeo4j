import { combineReducers, createStore, applyMiddleware } from "redux";
import createSagaMiddleware from "redux-saga";
import { all } from "@redux-saga/core/effects";
import { commentReducer } from "./comment/reducer";
import { userReducer } from "./user/reducer";
import { postReducer } from "./post/reducer";
import { uiReducer } from "./ui/reducer";
import { userSaga } from "./user/saga";
import { postSaga } from "./post/saga";
import { commentsSaga } from "./comment/saga";
import { composeWithDevTools } from 'redux-devtools-extension';
import { appSaga } from "./app/saga";
import { categorieReducer } from "./categorie/reducer";
import { categorieSaga } from "./categorie/saga";
import { messageReducer } from "./message/reducer";
import {messageSaga} from "./message/saga";

export const DATABASE_URL = "http://localhost:4000/";
export const API_URL = "https://localhost:5001/api/"; 
export const USERS_RESOURCE_URL = DATABASE_URL + "users/";
export const COMMENTS_RESOURCE_URL = DATABASE_URL + "comments/";
export const CATEGORIES_RESOURCE_URL = DATABASE_URL + "categories/";
export const POSTS_RESOURCE_URL = DATABASE_URL + "posts/";

export interface NormalizedObjects<T> {
  byId: { [id: number]: T },
  allIds: number[],
  isLoaded: boolean
}

export const rootReducer = combineReducers({
  posts: postReducer,
  comments: commentReducer,
  users: userReducer,
  ui: uiReducer,
  categories: categorieReducer,
  messages: messageReducer
});

export default function configureStore() {
  const sagaMiddleware = createSagaMiddleware();
  const store = createStore(
    rootReducer,
    composeWithDevTools(applyMiddleware(sagaMiddleware))
  );

  sagaMiddleware.run(rootSaga);
  return store;
}

export function* rootSaga() {
  yield all([
    appSaga(),
    userSaga(),
    postSaga(),
    commentsSaga(),
    categorieSaga(),
    messageSaga()
  ]);
}