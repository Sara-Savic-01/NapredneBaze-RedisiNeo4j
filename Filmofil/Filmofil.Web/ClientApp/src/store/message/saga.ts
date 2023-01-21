import * as saga from "redux-saga/effects";
import { API_URL, NormalizedObjects } from "..";
import { apiFetch } from "../../services/auth";
import normalize from "../../services/normalizer";
import { getLoggedUser } from "../app/saga";
import { getUsers } from "../post/saga";
import { openErrorDialog, stopSpinner } from "../ui/action";
import { UserState } from "../user/types";
import { loadMessagesAndSubscribe } from "./action";
import { MessageActionTypes, SendMessageAction } from "./types";
import {connection} from "../../services/hub-connection";

export function* messageSaga() {
  yield saga.all([saga.fork(watchRequests)]);
}

function* watchRequests() {
  yield saga.takeEvery(MessageActionTypes.FETCH_MESSAGES_AND_SUBSCRIBE, fetchMessages);
  yield saga.takeEvery(MessageActionTypes.INIT_SEND_MESSAGE, send);
}


function* fetchMessages():any {
  const userId = yield saga.select(getLoggedUser);
  const users: NormalizedObjects<UserState> = yield saga.select(getUsers);

  const input = {
    ConnectionId: connection.connectionId,
    Categories: users.byId[userId].categories
  }
  
  const result = yield apiFetch('POST', API_URL + "Chat/LoadChatDataAndSubscribe", input);

  if(result.success) {
    yield saga.put(loadMessagesAndSubscribe(
      normalize(result.data.messages), normalize(result.data.users)));
  }
  else {
    yield saga.put(openErrorDialog("", "Došlo je do greške, molimo Vas osvežte stranicu ponovo!"));
  }

  yield saga.put(stopSpinner());
}

function* send(action: SendMessageAction) {
  const {message} = (action as SendMessageAction);
  const input = {
    Id: 0,
    Sender: message.sender,
    Categorie: message.categorie,
    Content: message.content
  };
  
  yield apiFetch('POST', API_URL + "Chat/SendMessage", input);
  yield saga.put(stopSpinner());
}

export const getCategories = (state: any) => state.categories;