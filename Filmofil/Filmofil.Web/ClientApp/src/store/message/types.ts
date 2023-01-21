import { Action } from "redux";
import { NormalizedObjects } from "..";
import { UserState } from "../user/types";

export interface MessageState {
  id: number,
  sender: number,
  categorie: number,
  content: string
}

export enum MessageActionTypes {
  FETCH_MESSAGES_AND_SUBSCRIBE = "message/FETCH_MESSAGES_AND_SUBSCRIBE",
  LOAD_MESSAGES = "message/LOAD_MESSAGES",
  INIT_SEND_MESSAGE = "message/INIT_SEND_MESSAGE",
  SEND_MESSAGE = "message/SEND_MESSAGE"
}

export interface LoadMessagesAction extends Action {
  messages: NormalizedObjects<MessageState>,
  users: NormalizedObjects<UserState>
}

export interface SendMessageAction extends Action {
  message: MessageState
}