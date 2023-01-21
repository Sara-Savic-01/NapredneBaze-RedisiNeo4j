import { Button, FormControlLabel, Radio, RadioGroup } from '@material-ui/core';
import ArrowDownwardIcon from '@material-ui/icons/ArrowDownward';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import Header from '../components/header/Header';
import Post from '../components/post/Post';
import { NormalizedObjects } from '../store';
import { initLoadMorePosts } from '../store/post/action';
import { PostState } from '../store/post/types';
import { initSortPosts, startSpinner } from '../store/ui/action';
import { UiState } from '../store/ui/types';
import styles from "./css/home.module.css";

interface PropsFromState {
  ui: UiState,
  posts: NormalizedObjects<PostState>
}

interface PropsFromDispatch {
  initSortPosts: typeof initSortPosts,
  startSpinner: typeof startSpinner,
  initLoadMorePosts: typeof initLoadMorePosts
}

type allProps = PropsFromState & PropsFromDispatch;

class Home extends Component<allProps> {
  render() {
    return (
      <div>
        <Header isLoggedUser={this.props.ui.loggedUser === 0 ? false : true}></Header>
        <div className={styles.radioButtons}>
                <RadioGroup className={styles.radioButtons} color="primary" onChange={this.onSortChange}>
            <FormControlLabel 
              value="popular"
              control={<Radio color="primary" checked={this.props.ui.postsSortType === "popular"}></Radio>}
              label="Popularno">
            </FormControlLabel>
            <FormControlLabel 
              value="best"
              control={<Radio color="primary" checked={this.props.ui.postsSortType === "best"}></Radio>}
              label="Najbolje">
            </FormControlLabel>
            <FormControlLabel
              value="new"
              control={<Radio color="primary" checked={this.props.ui.postsSortType === "new"}></Radio>}
              label="Novo">
            </FormControlLabel>
          </RadioGroup>
        </div>
        <div className={styles.postsContainer}>
          {this.props.ui.homePosts.map(post => {
            return (
              <Post key={post}
                isOpened={this.props.ui.isOpenedSinglePost}
                postState={this.props.posts.byId[post]}
                cardWidthInPercentage="60%">
              </Post>
            )
          })}
        </div>
        <div className={styles.postsContainer}>
          <Button 
            className={styles.loadMoreButton} color="primary" variant="contained" style={{maxWidth: '200px', minWidth: '100px'}}
            onClick={this.loadMorePostsClick}>
            <ArrowDownwardIcon className={styles.loadMoreIcon} fontSize="inherit"/>
          </Button>
        </div>
      </div>
    );
  }

  onSortChange = (event: React.ChangeEvent<{}>, value: string) => {
    this.props.startSpinner();
    this.props.initSortPosts(value);
  }

  loadMorePostsClick = () => {
    this.props.startSpinner();
    this.props.initLoadMorePosts(this.props.ui.homePosts);
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
    initSortPosts: (sortType: string) => dispatch(initSortPosts(sortType)),
    startSpinner: () => dispatch(startSpinner()),
    initLoadMorePosts: (posts: number[]) => dispatch(initLoadMorePosts(posts))
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(Home);