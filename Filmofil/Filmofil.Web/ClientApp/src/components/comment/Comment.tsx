import { Avatar, Card, CardActions, CardContent, Collapse, IconButton, Typography } from '@material-ui/core';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import expandIcon from "../../assets/expand_icon.png";
import replyIcon from "../../assets/reply_icon.png";
import { CommentState } from '../../store/comment/types';
import { UserState } from '../../store/user/types';
import CommentForm from '../commentForm/CommentForm';
import Likes from '../post/Likes';
import styles from "./css/comment.module.css";
import { PostState } from '../../store/post/types';

interface PropsFromState {
  post: PostState
  commentState: CommentState,
  user: UserState
}

interface IProps {
  id: number
}

interface IState {
  expandComments: boolean,
  expandCommentForm: boolean
}

type allProps = PropsFromState & IProps;

class Comment extends Component<allProps, IState> {
  readonly state = {
    expandComments: false,
    expandCommentForm: false
  }
  
  render() {
    if(!this.props.commentState || !this.props.user || !this.props.post) return(<div></div>);
    
    return (
      <div>
        <Card className={styles.comment}>
          <CardContent className={styles.content}>
            <Typography className={styles.caption} variant="caption">
              Odgovorio/la {this.props.user.username}
            </Typography>
            <CardContent>
              <Typography variant="body1">{this.props.commentState.content}</Typography>
            </CardContent>
            <Collapse in={this.state.expandCommentForm}>
              <CommentForm 
                isParentComponentPost={false} 
                parentCommentId={this.props.commentState.id}
                postId={this.props.commentState.postId}>
              </CommentForm>
            </Collapse>
          </CardContent>
          <CardActions className={styles.commentSidebar}>
            <Likes 
              parentComponentId={this.props.commentState.id}
              likes={this.props.commentState.likesCount} 
              IsInCommentSection={true}
              parent={this.props.commentState}
              categorieTitle={this.props.post.categorieTitle}>
            </Likes>
            <IconButton
              className={styles.expandButton}
              aria-expanded={false}
              onClick={() => this.setState({ expandCommentForm: !this.state.expandCommentForm })}>
              <Avatar src={replyIcon}></Avatar>
            </IconButton>
            <IconButton
              className={styles.expandButton}
              aria-expanded={false}
              onClick={() => this.setState({expandComments: !this.state.expandComments})}>
              <Avatar src={expandIcon}></Avatar>
            </IconButton>
          </CardActions>
          <Collapse in={this.state.expandComments} unmountOnExit>
            {this.props.commentState.comments.map(comment => (
              <ConnectedComment key={comment} id={comment}></ConnectedComment>
            ))}
          </Collapse>
        </Card>
      </div>
    );
  }
}

const mapStateToProps = (rootReducer: any, ownProps: any) => {
  if(rootReducer.users.isLoaded && rootReducer.comments.isLoaded && rootReducer.posts.isLoaded)
    return {
      commentState: rootReducer.comments.byId[ownProps.id],
      user: rootReducer.users.byId[rootReducer.comments.byId[ownProps.id].authorId],
      post: rootReducer.posts.byId[rootReducer.comments.byId[ownProps.id].postId]
    }
  else 
    return {
      commentState: null,
      user: null,
      post: null
    }
}

const ConnectedComment = connect(mapStateToProps)(Comment);

export default ConnectedComment;