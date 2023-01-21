import { Action } from 'redux';
import { NormalizedObjects } from '..';

export interface CategorieState {
  id: number,
  title: string,
  users: number[],
  posts: number[],
  messages: number[]
}

export enum CategorieActionTypes {
  LOAD_CATEGORIES = "categorie/LOAD_CATEGORIES",
  LEAVE_CATEGORIE = "categorie/LEAVE_CATEGORIE",
  JOIN_CATEGORIE = "categorie/JOIN_CATEGORIE",
  INIT_LEAVE_CATEGORIE = "categorie/INIT_LEAVE_CATEGORIE",
  INIT_JOIN_CATEGORIE = "categorie/INIT_JOIN_CATEGORIE"
}

export interface LoadCategoriesAction extends Action {
  categories: NormalizedObjects<CategorieState>
}

export interface LeaveCategorieAction extends Action {
  categorie: number,
  user: number
}

export interface JoinCategorieAction extends Action {
  categorie: number,
  user: number
}

export interface InitLeaveCategorieAction extends Action {
  categorie: number,
  user: number
}

export interface InitJoinCategorieAction extends Action {
  categorie: number,
  user: number
}