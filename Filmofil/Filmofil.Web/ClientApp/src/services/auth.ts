import { NormalizedObjects } from "../store";
import { UserState } from "../store/user/types";

const MINIMUM_PASSWORD_LENGTH = 6;
const MINIMUM_USERNAME_LENGTH = 6;

export function* apiFetch(method: string, url: string, data: any):any {
  let postParams: RequestInit = {
    body: JSON.stringify(data),
    method: method,
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json, text/plain, */*'
    }
  }
  let getParams: RequestInit = {
    method: method,
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json, text/plain, */*'
    }
  }
  let result;
  if (data) result = yield fetch(url, postParams);
  else result = yield fetch(url, getParams);
  
  return yield result.json();
}

export function FilmofilFetch(method: string, url: string, data: any) : Promise<Response> {
  let postParams: RequestInit = {
    body: JSON.stringify(data),
    method: method,
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json, text/plain, */*'
    }
  }
  let getParams: RequestInit = {
    method: method,
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json, text/plain, */*'
    }
  }
  if (data) return fetch(url, postParams);
  return fetch(url, getParams);
}

export function validateEmail(email: string): boolean {
  const regexp = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  return regexp.test(email);
}

export function validatePassword(password: string): boolean {
  return password.length >= MINIMUM_PASSWORD_LENGTH;
}

export function validateUsername(username: string): boolean {
  return username.length >= MINIMUM_USERNAME_LENGTH;
}

export function getUserByEmail(users: NormalizedObjects<UserState>, email: string) {
  for(let index = 0; index < users.allIds.length; index++) {
    const id = users.allIds[index];
    if (users.byId[id].email === email){
      return users.byId[id];
  }};
  return null;
}