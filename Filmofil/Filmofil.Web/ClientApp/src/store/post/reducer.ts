import { Reducer } from "redux";
import { NormalizedObjects } from "..";
import { AppActionTypes, LoadCategorieDataAction, LoadDataAction } from "../app/types";
import { addTo, alreadyDisliked, alreadyLiked, removeFrom } from "../user/reducer";
import { DislikePostAction, LikePostAction, UserActionTypes, LoadUserDataAction } from "../user/types";
import { AddCommentToPostAction, AddPostAction, LoadPostsAction, PostActionTypes, PostState, LoadMorePostsAction } from "./types";

const initialState: NormalizedObjects<PostState> = {
  byId: {},
  allIds: [],
  isLoaded: false
}

const reducer: Reducer<NormalizedObjects<PostState>> = (state = initialState, action) => {
  switch (action.type) {
    case AppActionTypes.FETCH_DATA: { return state; }
    case PostActionTypes.ADD_POST: {
      const post = (action as AddPostAction).post;
      return {
        ...state, 
        byId: {
          ...state.byId,
          [post.id]: post
        },
        allIds: [...state.allIds, post.id]
      }
    }
    case AppActionTypes.LOAD_DATA: {
      return {
        ...(action as LoadDataAction).appState.posts,
        isLoaded: true
      }
    }
    case PostActionTypes.LOAD_MORE_POSTS: {
      return {
        ...state,
        byId: {
          ...state.byId,
          ...(action as LoadMorePostsAction).appState.posts.byId
        },
        allIds: [...state.allIds, ...(action as LoadMorePostsAction).appState.posts.allIds],
        isLoaded: true
      }
    }
    case UserActionTypes.LOAD_USER_DATA: {
      return {
        ...state,
        byId: {
          ...state.byId,
          ...(action as LoadUserDataAction).appState.posts.byId
        },
        allIds: [...state.allIds, ...(action as LoadUserDataAction).appState.posts.allIds],
        isLoaded: true
      }
    }
    case PostActionTypes.LOAD_MORE_CATEGORIE_POSTS: {
      return {
        ...state,
        byId: {
          ...state.byId,
          ...(action as LoadMorePostsAction).appState.posts.byId
        },
        allIds: [...state.allIds, ...(action as LoadMorePostsAction).appState.posts.allIds],
        isLoaded: true
      }
    }
    case AppActionTypes.LOAD_CATEGORIE_DATA: {
      return {
        ...state,
        byId: {
          ...state.byId,
          ...(action as LoadCategorieDataAction).appState.posts.byId
        },
        allIds: [...state.allIds, ...(action as LoadCategorieDataAction).appState.posts.allIds],
        isLoaded: true
      }
    }
    case PostActionTypes.ADD_COMMENT_TO_POST: {
      const {id, postId} = (action as AddCommentToPostAction).comment;
      return {
        ...state,
        byId: {
          ...state.byId,
          [postId as number]: {
            ...state.byId[postId as number],
            comments: [...state.byId[postId as number].comments, id]
          }
        }
      }
    }
    case UserActionTypes.LIKE_POST: {
      const {postId, userId} = (action as LikePostAction);
      let {likes, dislikes} = state.byId[postId];

      likes = alreadyLiked(likes, userId) ? 
        removeFrom(likes, userId) : addTo(likes, userId);

      dislikes = removeFrom(dislikes, userId);

      return setState(state, postId, likes, dislikes);
    }
    case UserActionTypes.DISLIKE_POST: {
      const {postId, userId} = (action as DislikePostAction);
      let {likes, dislikes} = state.byId[postId];

      dislikes = alreadyDisliked(dislikes, userId) ?
        removeFrom(dislikes, userId) : addTo(dislikes, userId);

      likes = removeFrom(likes, userId);
      
      return setState(state, postId, likes, dislikes);
    }
    case PostActionTypes.LOAD_POSTS: {
      const { posts } = (action as LoadPostsAction);
      return { ...state, posts }
    }
    default: return state;
  }
}

const setState = (state: any, postId: number, likes: number[], dislikes: number[]) => {
  return {
    ...state,
    byId: {
      ...state.byId,
      [postId]: {
        ...state.byId[postId],
        dislikes: dislikes,
        likes: likes,
        likesCount: likes.length - dislikes.length
      }
    }
  }
}

export { reducer as postReducer };

