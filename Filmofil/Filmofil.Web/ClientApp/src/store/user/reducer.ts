import { Reducer } from "redux";
import { NormalizedObjects } from "..";
import { AppActionTypes, LoadDataAction } from "../app/types";
import { CategorieActionTypes, JoinCategorieAction, LeaveCategorieAction } from "../categorie/types";
import { AddPostAction, PostActionTypes, AddCommentToPostAction, LoadMorePostsAction } from "../post/types";
import { DislikeCommentAction, DislikePostAction, LikeCommentAction, LikePostAction, SignUpAction, UserActionTypes, UserState, LoadUserDataAction } from "./types";
import { CommentActionTypes, ReplyToCommentAction } from "../comment/types";
import { MessageActionTypes, LoadMessagesAction, SendMessageAction } from "../message/types";

const initialState: NormalizedObjects<UserState> = {
  byId: {},
  allIds: [],
  isLoaded: false
}

const reducer: Reducer<NormalizedObjects<UserState>> = (state = initialState, action) => {
  switch (action.type) {
    case UserActionTypes.SIGN_UP: {
      const signUpData = (action as SignUpAction).signUpData;
      return {
        ...state,
        byId: {
          ...state.byId,
          [signUpData.id]: {
            ...state.byId[signUpData.id],
            id: signUpData.id,
            username: signUpData.username,
            email: signUpData.email,
            password: signUpData.password,
            posts: [],
            comments: [],
            likedPosts: [],
            dislikedPosts: [],
            likedComments: [],
            dislikedComments: [],
            categories: [],
            messages: []
          }
        },
        allIds: [ ...state.allIds, signUpData.id ]
      }
    }
    case AppActionTypes.LOAD_DATA: {
      return {
        ...state,
        byId: {
          ...state.byId,
          ...(action as LoadDataAction).appState.users.byId
        },
        allIds: [...state.allIds, ...(action as LoadDataAction).appState.users.allIds],
        isLoaded: true
      }
    }
    case UserActionTypes.LOAD_USER_DATA: {
      return {
        ...state,
        byId: {
          ...state.byId,
          ...(action as LoadUserDataAction).appState.users.byId
        },
        allIds: [...state.allIds, ...(action as LoadUserDataAction).appState.users.allIds],
        isLoaded: true
      }
    }
    case PostActionTypes.LOAD_MORE_POSTS: {
      return {
        ...state,
        byId: {
          ...state.byId,
          ...(action as LoadMorePostsAction).appState.users.byId
        },
        allIds: [...state.allIds, ...(action as LoadMorePostsAction).appState.users.allIds],
        isLoaded: true
      }
    }
    case PostActionTypes.LOAD_MORE_CATEGORIE_POSTS: {
      return {
        ...state,
        byId: {
          ...state.byId,
          ...(action as LoadMorePostsAction).appState.users.byId
        },
        allIds: [...state.allIds, ...(action as LoadMorePostsAction).appState.users.allIds],
        isLoaded: true
      }
    }
    case UserActionTypes.LIKE_POST: {
      const { userId, postId } = (action as LikePostAction);
      let {likedPosts, dislikedPosts} = state.byId[userId];

      likedPosts = alreadyLiked(likedPosts, postId) ? 
        removeFrom(likedPosts, postId) :
        addTo(likedPosts, postId);

      dislikedPosts = removeFrom(dislikedPosts, postId);

      return { ...state,
        byId: { ...state.byId,
          [userId]: { ...state.byId[userId], 
            likedPosts, 
            dislikedPosts
          }
        }
      }
    }
    case UserActionTypes.DISLIKE_POST: {
      const { userId, postId } = (action as DislikePostAction);
      let { likedPosts, dislikedPosts } = state.byId[userId];

      dislikedPosts = alreadyDisliked(dislikedPosts, postId) ?
        removeFrom(dislikedPosts, postId) :
        addTo(dislikedPosts, postId);

      likedPosts = removeFrom(likedPosts, postId);

      return { ...state,
        byId: { ...state.byId,
          [userId]: { ...state.byId[userId],
            dislikedPosts, likedPosts
          }
        }
      }
    }
    case UserActionTypes.LIKE_COMMENT: {
      const { userId, commentId } = (action as LikeCommentAction);
      let { likedComments, dislikedComments } = state.byId[userId];

      likedComments = alreadyLiked(likedComments, commentId) ?
        removeFrom(likedComments, commentId) :
        addTo(likedComments, commentId);

      dislikedComments = removeFrom(dislikedComments, commentId);

      return { ...state,
        byId: { ...state.byId,
          [userId]: { ...state.byId[userId],
            dislikedComments, likedComments
          }
        }
      }
    }
    case UserActionTypes.DISLIKE_COMMENT: {
      const { userId, commentId } = (action as DislikeCommentAction);
          let { likedComments, dislikedComments } = state.byId[userId];

      dislikedComments = alreadyDisliked(dislikedComments, commentId) ?
        removeFrom(dislikedComments, commentId) : 
        addTo(dislikedComments, commentId);

      likedComments = removeFrom(likedComments, commentId);

      return { ...state,
        byId: { ...state.byId,
          [userId]: { ...state.byId[userId],
            dislikedComments, likedComments
          }
        }
      }
    }
    case PostActionTypes.ADD_POST: {
      const post = (action as AddPostAction).post;
      return {
        ...state,
        byId: {
          ...state.byId,
          [post.authorId]: {
            ...state.byId[post.authorId],
            posts: [...state.byId[post.authorId].posts, post.id]
          }
        }
      }
    }
    case CommentActionTypes.REPLY_TO_COMMENT: {
      const comment = (action as ReplyToCommentAction).comment;
      return {
        ...state,
        byId: {
          ...state.byId,
          [comment.authorId]: {
            ...state.byId[comment.authorId],
            comments: [...state.byId[comment.authorId].comments, comment.id]
          }
        }
      }
    }
    case PostActionTypes.ADD_COMMENT_TO_POST: {
      const comment = (action as AddCommentToPostAction).comment;
      return {
        ...state,
        byId: {
          ...state.byId,
          [comment.authorId]: {
            ...state.byId[comment.authorId],
            comments: [...state.byId[comment.authorId].comments, comment.id]
          }
        }
      }
    }
    case CategorieActionTypes.JOIN_CATEGORIE: {
      const {user, categorie} = (action as JoinCategorieAction);
      return {
        ...state,
        byId: {
          ...state.byId,
          [user]: {
            ...state.byId[user],
            categories: [...state.byId[user].categories, categorie]
          }
        }
      }
    }
    case CategorieActionTypes.LEAVE_CATEGORIE: {
      const {user, categorie} = (action as LeaveCategorieAction);
      let categories = removeFrom(state.byId[user].categories, categorie);
      return {
        ...state,
        byId: {
          ...state.byId,
          [user]: {
            ...state.byId[user],
            categories
          }
        }
      }
    }
    case MessageActionTypes.LOAD_MESSAGES: {
      const {users} = (action as LoadMessagesAction);
      return {
        ...state,
        allIds: [...state.allIds, ...users.allIds],
        byId: {
          ...state.byId,
          ...users.byId
        }
      }
    }
    case MessageActionTypes.SEND_MESSAGE: {
      const {message} = (action as SendMessageAction);
      return {
        ...state,
        byId: {
          ...state.byId,
          [message.sender]: {
            ...state.byId[message.sender],
            messages: [...state.byId[message.sender].messages, message.id]
          }
        }
      }
    }
    default: return state;
  }
}

export const removeFrom = (source: any[], target: any): any[] => {
  return source.filter(id => id !== target)
}

export const addTo = (source: any[], data: any): any[] => {
  return [...source, data];
}

export const alreadyDisliked = (source: any[], target: any): boolean => {
  return source.includes(target);
}

export const alreadyLiked = (source: any[], target: any): boolean => {
  return source.includes(target);
}

export { reducer as userReducer };

