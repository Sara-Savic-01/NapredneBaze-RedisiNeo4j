import * as signalR from "@microsoft/signalr";
import * as saga from "redux-saga/effects";
import { sendMessage } from "../store/message/action";
import { getLoggedUser } from "../store/app/saga";
import { UserState } from "../store/user/types";
import { NormalizedObjects, API_URL } from "../store";
import { getUsers } from "../store/post/saga";
import { apiFetch } from "./auth";
import { store } from "..";

Object.defineProperty(WebSocket, 'OPEN', { value: 1, });

export const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:5001/chat", { transport: 1 })
  .configureLogging(signalR.LogLevel.Trace)
  .withAutomaticReconnect()
  .build();

connection.start();

connection.on("SendMessage", (data) => {
  store.dispatch(sendMessage(data));
});

connection.onreconnected(function*() {
  const userId = yield saga.select(getLoggedUser);
  const users: NormalizedObjects<UserState> = yield saga.select(getUsers);

  const input = {
    ConnectionId: connection.connectionId,
    Categories: users.byId[userId].categories
  }
  
  yield apiFetch('POST', API_URL + "Chat/LoadChatDataAndSubscribe", input);
});