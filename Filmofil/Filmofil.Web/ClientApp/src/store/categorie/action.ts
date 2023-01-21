import { NormalizedObjects } from '..';
import { CategorieActionTypes, CategorieState, JoinCategorieAction, LeaveCategorieAction, LoadCategoriesAction, InitLeaveCategorieAction, InitJoinCategorieAction } from './types';

export function leaveCategorie(categorie: number, user: number): LeaveCategorieAction {
  return {type: CategorieActionTypes.LEAVE_CATEGORIE, categorie: categorie, user: user}
}

export function joinCategorie(categorie: number, user: number): JoinCategorieAction {
  return {type: CategorieActionTypes.JOIN_CATEGORIE, categorie: categorie, user: user}
}

export function loadCategories(categories: NormalizedObjects<CategorieState>): LoadCategoriesAction {
  return { type: CategorieActionTypes.LOAD_CATEGORIES, categories: categories }
}

export function initLeaveCategorie(categorie: number, user: number): InitLeaveCategorieAction {
  return {type: CategorieActionTypes.INIT_LEAVE_CATEGORIE, categorie: categorie, user: user}
}

export function initJoinCategorie(categorie: number, user: number): InitJoinCategorieAction {
  return {type: CategorieActionTypes.INIT_JOIN_CATEGORIE, categorie: categorie, user: user}
}