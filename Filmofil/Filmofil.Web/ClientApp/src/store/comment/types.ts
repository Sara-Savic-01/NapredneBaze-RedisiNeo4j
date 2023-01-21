import { Action } from "redux";
import { NormalizedObjects } from "..";

export interface CommentState {
  id: number,
  authorId: number,
  postId: number,
  parentCommentId: number | null,
  content: string,
  likes: number[],
  dislikes: number[],
  likesCount: number,
  comments: number[],
}

export enum CommentActionTypes {
  LOAD_COMMENTS = "comment/LOAD_COMMENTS",
  LOAD_COMMENTS_SUCCESS = "comment/LOAD_COMMENTS_SUCCESS",
  REPLY_TO_COMMENT = "comment/REPLY_TO_COMMENT",
  INIT_REPLY_TO_COMMENT = "comment/INIT_REPLY_TO_COMMENT",
}

export interface LoadCommentsSuccessAction extends Action {
  comments: NormalizedObjects<CommentState>
}

export interface ReplyToCommentAction extends Action {
  comment: CommentState
}

export interface InitReplyToCommentAction extends Action {
  comment: CommentState, categorieTitle: string
}

export interface LoadCommentsAction extends Action {
  comments: NormalizedObjects<CommentState>
}