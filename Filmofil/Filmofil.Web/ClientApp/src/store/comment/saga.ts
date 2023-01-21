import * as saga from "redux-saga/effects";
import { API_URL } from "..";
import { apiFetch } from "../../services/auth";
import { openErrorDialog, stopSpinner } from "../ui/action";
import { DislikeCommentAction, LikeCommentAction, UserActionTypes } from "../user/types";
import { replyToComment } from "./action";
import { CommentActionTypes,  InitReplyToCommentAction } from "./types";

export function* commentsSaga() {
  yield saga.all([saga.fork(watchFetchRequest)]);
}

function* watchFetchRequest() {
  yield saga.takeEvery(CommentActionTypes.INIT_REPLY_TO_COMMENT, reply);
  yield saga.takeEvery(UserActionTypes.LIKE_COMMENT, likeDislikeUpdate);
  yield saga.takeEvery(UserActionTypes.DISLIKE_COMMENT, likeDislikeUpdate);
}

function* reply(action: InitReplyToCommentAction):any {
  const input = {
    Content: action.comment.content,
    PostId: action.comment.postId,
    AuthorId: action.comment.authorId,
    ParentCommentId: action.comment.parentCommentId,
    CategorieTitle: action.categorieTitle
  }
  const result = yield apiFetch('POST', API_URL + "Comment/ReplyToComment", input);

  if(result.success) {
    yield saga.put(replyToComment({...action.comment, id: result.data}));
  }
  else {
    yield saga.put(openErrorDialog("", "Došlo je do greške, pokušajte ponovo!"));
  }

  yield saga.put(stopSpinner());
}

function* likeDislikeUpdate(action: LikeCommentAction | DislikeCommentAction):any {
  if(action.type === UserActionTypes.LIKE_COMMENT) {
    return yield apiFetch('POST', API_URL + "Comment/LikeComment/?commentId=" 
    + action.commentId + "&userId=" + action.userId + "&categorieTitle=" + action.categorieTitle, "");
  }
  else {
    return yield apiFetch('POST', API_URL + "Comment/DislikeComment/?commentId=" 
    + action.commentId + "&userId=" + action.userId + "&categorieTitle=" + action.categorieTitle, "");
  }
}

export const getComments = (state: any) => state.comments;