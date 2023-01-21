import { Button, TextField } from '@material-ui/core';
import React, { ChangeEvent, Component } from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { NormalizedObjects } from '../../store';
import { initReplyToComment } from '../../store/comment/action';
import { CommentState } from '../../store/comment/types';
import { initAddCommentToPost } from '../../store/post/action';
import { openLoginDialog, startSpinner } from '../../store/ui/action';
import { UiState } from '../../store/ui/types';
import { UserState } from '../../store/user/types';
import styles from "./css/commentForm.module.css";
import { PostState } from '../../store/post/types';

interface IProps {
  isParentComponentPost: boolean,
  parentCommentId: number | null,
  postId: number
}

interface propsFromState {
  ui: UiState,
  users: NormalizedObjects<UserState>,
  posts: NormalizedObjects<PostState>
}

interface propsFromDispatch {
  openLoginDialog: typeof openLoginDialog,
  initAddCommentToPost: typeof initAddCommentToPost,
  initReplyToComment: typeof initReplyToComment,
  startSpinner: typeof startSpinner
}

interface IState {
  content: string,
  loggedUserUsername: string
}

type allProps = IProps & propsFromState & propsFromDispatch;

class CommentForm extends Component<allProps, IState> {
  readonly state = {
    content: "",
    loggedUserUsername: this.getLoggedUserUsername()
  }

  render() {
    const {loggedUser} = this.props.ui;
    const user: UserState = this.props.users.byId[loggedUser];
    return (
      <div className={styles.comment}>
        <TextField
          id="outlined-multiline-static"
          label={loggedUser === 0 ? "Komentar" : "" + user.username}
          placeholder={"Unesite Vaš komentar ovde"}
          value={this.state.content} onChange={this.onContentChange}
          multiline fullWidth margin="normal" variant="outlined"
        />
        <Button onClick={this.onCommentButtonClick} variant="contained" color={"primary"} size={"small"} style={{borderRadius: "10px"}}>Komentar</Button>
      </div>
    );
  }

  getLoggedUserUsername(): string {
    if(this.props.ui.loggedUser !== 0) {
      return this.props.users.byId[this.props.ui.loggedUser].username;
    }
    return "";
  }

  onCommentButtonClick = () => {
    if(this.props.ui.loggedUser === 0){
      this.props.openLoginDialog();
    } 
    else if(this.state.content !== "") {
      const comment: CommentState = {
        authorId: this.props.ui.loggedUser,
        comments: [],
        content: this.state.content,
        id: 0,
        likes: [],
        dislikes: [],
        likesCount: 0,
        parentCommentId: this.props.isParentComponentPost ? null : this.props.parentCommentId,
        postId: this.props.postId
      }
      this.props.startSpinner();
      this.props.isParentComponentPost ? 
        this.props.initAddCommentToPost(comment) : 
        this.props.initReplyToComment(comment, this.props.posts.byId[this.props.postId].categorieTitle);
      this.setState({content: ""});
    }
  }

  onContentChange = (event: ChangeEvent<HTMLInputElement>) => {
    this.setState({content: event.currentTarget.value});
  }
}


const mapStateToProps = (rootReducer: any) => {
  return {
    ui: rootReducer.ui,
    users: rootReducer.users,
    posts: rootReducer.posts
  }
}

const mapDispatchToProps = (dispatch: Dispatch) => {
  return {
    openLoginDialog: () => dispatch(openLoginDialog()),
    initAddCommentToPost: (comment: CommentState) => dispatch(initAddCommentToPost(comment)),
    initReplyToComment: (comment: CommentState, categorieTitle: string) => 
      dispatch(initReplyToComment(comment, categorieTitle)),
    startSpinner: () => dispatch(startSpinner())
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(CommentForm);