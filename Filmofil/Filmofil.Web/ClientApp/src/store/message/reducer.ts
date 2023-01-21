import { MessageState, MessageActionTypes, LoadMessagesAction, SendMessageAction } from "./types";
import { NormalizedObjects } from "./../index";
import { Reducer } from "redux";

const initialState: NormalizedObjects<MessageState> = {
  byId: {},
  allIds: [],
  isLoaded: false
}

const reducer: Reducer<NormalizedObjects<MessageState>> = (state = initialState, action) => {
  switch(action.type) {
    case MessageActionTypes.LOAD_MESSAGES: {
      const messages = (action as LoadMessagesAction).messages;
      return {
        ...state,
        byId: {
          ...state.byId,
          ...messages.byId
        },
        allIds: [...state.allIds, ...messages.allIds],
        isLoaded: true
      }
    }
    case MessageActionTypes.SEND_MESSAGE: {
      const { message } = (action as SendMessageAction);
      return {
        ...state,
        allIds: [...state.allIds, message.id],
        byId: {
          ...state.byId,
          [message.id]: message
        }
      }
    }
    default: return state;
  }
}

export { reducer as messageReducer }