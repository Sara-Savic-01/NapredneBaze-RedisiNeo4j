import { Button, FormControlLabel, Radio, RadioGroup } from '@material-ui/core';
import ArrowDownwardIcon from '@material-ui/icons/ArrowDownward';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import Header from '../components/header/Header';
import Post from '../components/post/Post';
import { NormalizedObjects } from '../store';
import { initLoadMoreCategoriePosts } from '../store/post/action';
import { PostState } from '../store/post/types';
import { initSortCategoriePosts, startSpinner } from '../store/ui/action';
import { UiState } from '../store/ui/types';
import styles from "./css/home.module.css";
import { RouteComponentProps } from 'react-router';

interface IProps {
  categorieTitle: string
}

interface PropsFromState {
  ui: UiState,
  posts: NormalizedObjects<PostState>
}

interface PropsFromDispatch {
  sortCategoriePosts: typeof initSortCategoriePosts, 
  startSpinner: typeof startSpinner,
  initLoadMoreCategoriePosts: typeof initLoadMoreCategoriePosts 
}

type allProps = PropsFromState & PropsFromDispatch & IProps & RouteComponentProps<{ title: string }>;

interface IState {
  sortType: string
}

class Categorie extends Component<allProps, IState> {
  readonly state = {
    sortType: "popular"
  }

  componentDidMount() {
    if(this.props.ui.categoriePosts.length === 0)
      this.props.sortCategoriePosts(this.state.sortType, this.props.match.params.title);
  }

  render() {
    return (
      <div>
        <Header isLoggedUser={this.props.ui.loggedUser === 0 ? false : true}></Header>
        <div className={styles.radioButtons}>
          <RadioGroup className={styles.radioButtons} onChange={this.onSortChange}>
            <FormControlLabel 
              value="popular"
              control={<Radio color="primary" checked={this.state.sortType === "popular"}></Radio>}
              label="Popularno">
            </FormControlLabel>
            <FormControlLabel 
              value="best"
              control={<Radio color="primary" checked={this.state.sortType === "best"}></Radio>}
              label="Najbolje">
            </FormControlLabel>
            <FormControlLabel
              value="new"
              control={<Radio color="primary" checked={this.state.sortType === "new"}></Radio>}
              label="Novo">
            </FormControlLabel>
          </RadioGroup>
        </div>
        <div className={styles.postsContainer}>
          {this.props.ui.categoriePosts.map(post => {
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
    this.setState({...this.state, sortType: value});
    this.props.sortCategoriePosts(value, this.props.match.params.title);
  }

  loadMorePostsClick = () => {
    this.props.startSpinner();
    this.props.initLoadMoreCategoriePosts(
      this.props.ui.homePosts, this.state.sortType, this.props.match.params.title);
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
    sortCategoriePosts: (sortType: string, categorie: string) => 
      dispatch(initSortCategoriePosts(sortType, categorie)),
    startSpinner: () => dispatch(startSpinner()),
    initLoadMoreCategoriePosts: (posts: number[], category: string, categorie: string) => 
      dispatch(initLoadMoreCategoriePosts(posts, category, categorie))
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(Categorie);