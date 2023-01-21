import React, { Component } from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps, withRouter } from 'react-router';
import { Dispatch } from 'redux';
import Header from '../components/header/Header';
import Post from '../components/post/Post';
import { NormalizedObjects } from '../store';
import { PostState } from '../store/post/types';
import { UiState } from '../store/ui/types';
import styles from "./css/home.module.css";

interface PropsFromState {
  ui: UiState,
  posts: NormalizedObjects<PostState>
}

interface PropsFromDispatch {
}

type allProps = PropsFromState & PropsFromDispatch &
  RouteComponentProps<{ id: string }>;

class OpenedPost extends Component<allProps> {
  render() {
    return (
      <div>
        <Header isLoggedUser={this.props.ui.loggedUser === 0 ? false : true}></Header>
        <div className={styles.postsContainer}>
          {this.renderPost()}
        </div>
      </div>
    );
  }

  renderPost = () => {
    const id : number = parseFloat(this.props.match.params.id);
    if(this.props.posts.isLoaded && this.props.posts.allIds.find(post => id === post) != null) {
      return (
      <Post 
        key={id}
        isOpened={true}
        postState={this.props.posts.byId[id]}
        cardWidthInPercentage="60%">
      </Post>);
    }

    return(<div></div>);
  }

  isStoreLoaded = (): boolean => {
    return this.props.posts.allIds.length === 0 ? false : true;
  }
}


const mapStateToProps = (rootReducer: any) => {
  return {
    ui: rootReducer.ui,
    posts: rootReducer.posts
  }
}

const mapDispatchToProps = (dispatch: Dispatch) => {
  return {
  }
}

export default withRouter(connect(
  mapStateToProps, mapDispatchToProps
)(OpenedPost));