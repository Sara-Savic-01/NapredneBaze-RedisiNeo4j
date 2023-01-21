import * as saga from "redux-saga/effects";
import { API_URL } from "..";
import { apiFetch } from "../../services/auth";
import normalize from "../../services/normalizer";
import { AppActionTypes } from "../app/types";
import { openErrorDialog, stopSpinner } from "../ui/action";
import { loadData } from "./action";

export function* appSaga() {
  yield saga.all([saga.fork(watchRequests)]);
}

function* watchRequests() {
  yield saga.takeLatest(AppActionTypes.FETCH_DATA, fetchData);
}

function* fetchData():any {
  const category = yield saga.select(getCategory);
  const userId = yield saga.select(getLoggedUser);
  
  let result;
  if(userId === 0)
    result  = yield apiFetch('GET', API_URL + "App/GetClientState/?category=" + category, "");
  else result = yield apiFetch('GET', API_URL + "App/GetClientState/?category=" + category + "&userId=" + userId, "");

  if(result.success) {
    console.log(result);
    const appData = {
      users: normalize(result.data.users),
      posts: normalize(result.data.posts),
      comments: normalize(result.data.comments),
      categories: normalize(result.data.categories)
    }
    console.log(result.data);
    yield saga.put(loadData(appData));
  }
  else {
    yield saga.put(openErrorDialog("", "Došlo je do greške, pritisnite F5!"));
  }

  yield saga.put(stopSpinner());
} 

export const getCategory = (state: any) => state.ui.postsSortType;
export const getLoggedUser = (state: any) => state.ui.loggedUser;