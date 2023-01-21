import { Action } from "redux";
import { NormalizedObjects } from "..";
import { CommentState } from "../comment/types";
import { CategorieState } from "../categorie/types";
import { PostState } from "../post/types";
import { UserState } from "../user/types";

export enum AppActionTypes {
  FETCH_DATA = "app/FETCH_DATA",
  LOAD_DATA = "app/LOAD_DATA",
  LOAD_CATEGORIE_DATA = "app/LOAD_CATEGORIE_DATA"
}

export interface AppState {
  users: NormalizedObjects<UserState>,
  posts: NormalizedObjects<PostState>,
  comments: NormalizedObjects<CommentState>,
  categories: NormalizedObjects<CategorieState>
}

export interface LoadDataAction extends Action {
  appState: AppState
}

export interface LoadCategorieDataAction extends Action {
  appState: AppState
}