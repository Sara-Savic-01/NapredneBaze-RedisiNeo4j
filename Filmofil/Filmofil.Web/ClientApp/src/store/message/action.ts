import { MessageActionTypes, MessageState, SendMessageAction, LoadMessagesAction } from "./types";
import { NormalizedObjects } from "..";
import { UserState } from "../user/types";

export function fetchMessagesAndSubscribe() {
  return { type: MessageActionTypes.FETCH_MESSAGES_AND_SUBSCRIBE }
}

export function loadMessagesAndSubscribe(
  messages: NormalizedObjects<MessageState>, users: NormalizedObjects<UserState>): LoadMessagesAction 
{
  return { type: MessageActionTypes.LOAD_MESSAGES, messages, users }
}

export function sendMessage(message: MessageState): SendMessageAction {
  return { type: MessageActionTypes.SEND_MESSAGE, message }
}

export function initSendMessage(message: MessageState): SendMessageAction {
  return { type: MessageActionTypes.INIT_SEND_MESSAGE, message }
}