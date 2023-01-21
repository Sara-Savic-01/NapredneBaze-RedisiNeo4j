import * as saga from "redux-saga/effects";
import { API_URL } from "..";
import { apiFetch } from "../../services/auth";
import { openErrorDialog, stopSpinner } from "../ui/action";
import { joinCategorie, leaveCategorie } from "./action";
import { CategorieActionTypes, JoinCategorieAction, LeaveCategorieAction } from "./types";

export function* categorieSaga() {
  yield saga.all([saga.fork(watchFetchRequests)]);
}

function* watchFetchRequests() {
  yield saga.takeEvery(CategorieActionTypes.INIT_JOIN_CATEGORIE, join);
  yield saga.takeEvery(CategorieActionTypes.INIT_LEAVE_CATEGORIE, leave);
} 

function* join(action: JoinCategorieAction):any {
  const input = {
    UserId: action.user,
    CategorieId: action.categorie
  }
  const result = yield apiFetch('POST', API_URL + "User/JoinToCategorie", input);
  console.log("call");
  if(result.success) {
    yield saga.put(joinCategorie(action.categorie, action.user));
  }
  else {
    yield saga.put(openErrorDialog("", "Došlo je do greške, pokušajte ponovo!"));
  }

  yield saga.put(stopSpinner());
}

function* leave(action: LeaveCategorieAction):any {
  const input = {
    UserId: action.user,
    CategorieId: action.categorie
  }
  const result = yield apiFetch('POST', API_URL + "User/LeaveCategorie", input);

  if(result.success) {
    yield saga.put(leaveCategorie(action.categorie, action.user));
  }
  else {
    yield saga.put(openErrorDialog("", "Došlo je do greške, pokušajte ponovo!"));
  }

  yield saga.put(stopSpinner());
}