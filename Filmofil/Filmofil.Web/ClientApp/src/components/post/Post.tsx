import { ButtonBase, Card, CardActions, CardContent } from '@material-ui/core';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Redirect, withRouter } from 'react-router-dom';
import { NormalizedObjects } from '../../store';
import { CategorieState } from '../../store/categorie/types';
import { PostState } from '../../store/post/types';
import { UserState } from '../../store/user/types';
import Comment from '../comment/Comment';
import CommentForm from '../commentForm/CommentForm';
import styles from "./css/post.module.css";
import Likes from './Likes';
import PostContent from './PostContent';
import PostHeader from './PostHeader';

interface IProps {
  postState: PostState,
  isOpened: boolean,
  cardWidthInPercentage: string 
}

interface IState {
  redirect: boolean;
}

interface PropsFromState {
  user: UserState,
  categories: NormalizedObjects<CategorieState>
}

type allProps = IProps & PropsFromState;

class Post extends Component<allProps, IState> {
  readonly state = {
    redirect: false
  }

  render() {
    if(this.state.redirect) {
      return <Redirect push to={"/post/" + this.props.postState.id} />
    }
    console.log(this.props.user);
    if(!this.props.user) return(<div></div>);
    return (
      <div className={styles.post} style={{width: this.props.cardWidthInPercentage }}>
        <Card className={styles.postCard}>
          <CardActions className={styles.postSidebar}>
            <Likes 
              likes={this.props.postState.likesCount}
              IsInCommentSection={false}
              parentComponentId={this.props.postState.id}
              parent={this.props.postState}
              categorieTitle={this.props.postState.categorieTitle}>
            </Likes>
          </CardActions>
          {this.props.isOpened ? this.renderOpenedPost() : this.renderPost()}
        </Card>
        {this.props.isOpened ? this.renderComments() : ""}
      </div>
    );
  }

  renderPost = () => {
    return (
      <ButtonBase disableRipple={this.props.isOpened} onClick={this.onPostClick} className={styles.buttonBase}>
        {this.renderCardContent()}
      </ButtonBase>);
  }

  renderOpenedPost = () => {
      return(this.renderCardContent());
  }

  renderCardContent = () => {
    return (
      <CardContent className={styles.cardContent}>
        <PostHeader
          author={this.props.user.username}
          topic={this.props.categories.byId[this.props.postState.categorie].title}>
        </PostHeader>
        <CardContent>
          <PostContent content={this.props.postState.content}></PostContent>
        </CardContent>
        {this.props.isOpened ? 
          (<CommentForm 
            isParentComponentPost={true} 
            parentCommentId={null}
            postId={this.props.postState.id}>
          </CommentForm>
          ) : ""}
      </CardContent>
    );
  }

  renderComments = () => {
    return (this.props.postState.comments.map(comment => {
      return(
        <Comment id={comment} key={comment}></Comment>
      );
    }));
  }

  onPostClick = () => {
    this.setState({redirect: !this.props.isOpened});
  }
}

const mapStateToProps = (rootReducer: any, ownProps: any) => {
  console.log(rootReducer);
  console.log(ownProps);
  return {
    user: rootReducer.users.byId[ownProps.postState.authorId],
    categories: rootReducer.categories
  }
}

export default withRouter(connect(mapStateToProps)(Post));