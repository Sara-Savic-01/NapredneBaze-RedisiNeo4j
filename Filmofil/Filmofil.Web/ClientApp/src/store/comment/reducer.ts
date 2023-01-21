import { Reducer } from "redux";
import { NormalizedObjects } from "..";
import { AppActionTypes, LoadCategorieDataAction, LoadDataAction } from "../app/types";
import { AddCommentToPostAction, PostActionTypes, LoadMorePostsAction } from "../post/types";
import { addTo, alreadyDisliked, alreadyLiked, removeFrom } from "../user/reducer";
import { DislikeCommentAction, LikeCommentAction, UserActionTypes, LoadUserDataAction } from "../user/types";
import { CommentActionTypes, CommentState, ReplyToCommentAction } from "./types";

const initialState: NormalizedObjects<CommentState> = {
  byId: {},
  allIds: [],
  isLoaded: false
}

const reducer: Reducer<NormalizedObjects<CommentState>> = (state = initialState, action) => {
  switch (action.type) {
    case AppActionTypes.FETCH_DATA: { return state; }
    case AppActionTypes.LOAD_DATA: {
      return {
        ...(action as LoadDataAction).appState.comments,
        isLoaded: true
      }
    }
    case AppActionTypes.LOAD_CATEGORIE_DATA: {
      return {
        ...(action as LoadCategorieDataAction).appState.comments,
        isLoaded: true
      }
    }
    case PostActionTypes.LOAD_MORE_POSTS: {
      return {
        ...state,
        byId: {
          ...state.byId,
          ...(action as LoadMorePostsAction).appState.comments.byId
        },
        allIds: [...state.allIds, ...(action as LoadMorePostsAction).appState.comments.allIds],
        isLoaded: true
      }
    }
    case PostActionTypes.LOAD_MORE_CATEGORIE_POSTS: {
      return {
        ...state,
        byId: {
          ...state.byId,
          ...(action as LoadMorePostsAction).appState.comments.byId
        },
        allIds: [...state.allIds, ...(action as LoadMorePostsAction).appState.comments.allIds],
        isLoaded: true
      }
    }
    case UserActionTypes.LOAD_USER_DATA: {
      return {
        ...state,
        byId: {
          ...state.byId,
          ...(action as LoadUserDataAction).appState.comments.byId
        },
        allIds: [...state.allIds, ...(action as LoadUserDataAction).appState.comments.allIds],
        isLoaded: true
      }
    }
    case PostActionTypes.ADD_COMMENT_TO_POST: {
      const comment = (action as AddCommentToPostAction).comment;
      return {
        ...state,
        byId: {
          ...state.byId,
          [comment.id]: comment
        },
        allIds: [...state.allIds, comment.id]
      }
    }
    case CommentActionTypes.REPLY_TO_COMMENT: {
      const comment = (action as ReplyToCommentAction).comment;
      const parentCommentId: number = comment.parentCommentId as number;
      return {
        ...state,
        byId: {
          ...state.byId,
          [parentCommentId]: {
            ...state.byId[parentCommentId],
            comments: [...state.byId[parentCommentId].comments, comment.id]
          },
          [comment.id]: comment
        },
        allIds: [...state.allIds, comment.id]
      }
    }
    case UserActionTypes.LIKE_COMMENT: {
      const { commentId, userId } = (action as LikeCommentAction);
      let {likes, dislikes} = state.byId[commentId];

      likes = alreadyLiked(likes, userId) ?
        removeFrom(likes, userId) : addTo(likes, userId);

      dislikes = removeFrom(dislikes, userId);

      return setState(state, commentId, likes, dislikes);
    }
    case UserActionTypes.DISLIKE_COMMENT: {
      const { commentId, userId } = (action as DislikeCommentAction);
      let {likes, dislikes} = state.byId[commentId];

      dislikes = alreadyDisliked(dislikes, userId) ?
        removeFrom(dislikes, userId) : addTo(dislikes, userId);

      likes = removeFrom(likes, userId);

      return setState(state, commentId, likes, dislikes);
    }
    default: return state;
  }
}

const setState = (state:any, commentId: number, likes: number[], dislikes: number[]) => {
  return {
    ...state,
    byId: {
      ...state.byId,
      [commentId]: {
        ...state.byId[commentId],
        dislikes: dislikes,
        likes: likes,
        likesCount: likes.length - dislikes.length
      }
    }
  }
}

export { reducer as commentReducer };

