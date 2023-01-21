import * as saga from "redux-saga/effects";
import { API_URL } from "..";
import { apiFetch } from "../../services/auth";
import { LOGGED_USER_KEY, setItemToLocalStorage } from "../../services/local-storage";
import { fetchData, loadData } from "../app/action";
import { closeLoginDialog, closeSignupDialog, openErrorDialog, setLoggedUser, stopSpinner } from "../ui/action";
import { signUp, loadUserData } from "./action";
import { InitSignUpAction, InitSignUpData, LoginAction, UserActionTypes } from "./types";
import { getLoggedUser, getCategory } from "../app/saga";
import normalize from "../../services/normalizer";
import { UiActionTypes } from "../ui/types";

export function* userSaga() {
  yield saga.all([saga.fork(watchRequests)]);
}

function* watchRequests() {
  yield saga.takeEvery(UserActionTypes.INIT_SIGN_UP, initSignUp);
  yield saga.takeEvery(UserActionTypes.LOGIN, login);
  yield saga.takeEvery(UiActionTypes.LOGOUT_USER, logout);
  yield saga.takeEvery(UserActionTypes.FETCH_USER_DATA, fetchUserData)
}

function* initSignUp(action: InitSignUpAction):any {
  const data : InitSignUpData = action.signUpData;
  const createUserInput = {
    Email: data.email,
    Password: data.password,
    Username: data.username
  }
  
  const result = yield apiFetch('POST', API_URL + "User/CreateUser", createUserInput);

  if(result.success) {
    yield saga.all([
      saga.put(signUp({...data, id: result.data})),
      saga.put(setLoggedUser(result.data)),
      saga.put(closeSignupDialog())
    ]);
    setItemToLocalStorage<number>(LOGGED_USER_KEY, result.data);
    saga.put(fetchData());
  }
  else {
    yield saga.put(openErrorDialog("", "Vaš email i/ili korisničko ime je već zauzeto!"));
  }

  yield saga.put(stopSpinner());
}

function* login(action: LoginAction):any {
  const loginInput = {
    Username: action.username,
    Password: action.password
  }

  const result = yield apiFetch('POST', API_URL + "User/LogIn", loginInput);
  if(result.success) {
    yield saga.all([
      saga.put(setLoggedUser(result.data.id)),
      saga.put(closeLoginDialog())
    ]);
    setItemToLocalStorage<number>(LOGGED_USER_KEY, result.data.id);
    yield saga.put(fetchData());
  }
  else {
    yield saga.put(openErrorDialog("", "Pogrešno korisničko ime ili šifra!"));
  }

  yield saga.put(stopSpinner());
}

function* fetchUserData() :any{
  const userId = yield saga.select(getLoggedUser);

  const result = yield apiFetch('GET', API_URL + "User/GetUserPosts/?userId=" + userId, "");
  console.log(result.data);
  if(result.success) {
    const appData = {
      users: normalize(result.data.users),
      posts: normalize(result.data.posts),
      comments: normalize(result.data.comments),
      categories: normalize(null)
    }
    yield saga.put(loadUserData(appData));
  }
  else {
    yield saga.put(openErrorDialog("", "Došlo je do greške, pritisnite F5!"));
  }

  yield saga.put(stopSpinner());
}

function* logout():any {
  const category = yield saga.select(getCategory);
  
  const result = yield apiFetch('GET', API_URL + "App/GetClientState/?category=" + category, "");

  if(result.success) {
    const appData = {
      users: normalize(result.data.users),
      posts: normalize(result.data.posts),
      comments: normalize(result.data.comments),
      categories: normalize(result.data.categories)
    }
    yield saga.put(loadData(appData));
  }
  else {
    yield saga.put(openErrorDialog("", "Došlo je do greške, pritisnite F5!"));
  }

  yield saga.put(stopSpinner());
}