import { AppActionTypes, AppState, LoadDataAction, LoadCategorieDataAction } from "./types";

export function fetchData() { return {type: AppActionTypes.FETCH_DATA }}

export function loadData(appState: AppState) : LoadDataAction {
  return { type: AppActionTypes.LOAD_DATA, appState: appState }
}

export function loadCategorieData(appState: AppState): LoadCategorieDataAction {
  return { type: AppActionTypes.LOAD_CATEGORIE_DATA, appState }
}