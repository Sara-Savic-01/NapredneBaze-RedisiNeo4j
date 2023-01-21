import React, { Component } from 'react';
import { connect } from 'react-redux';
import { BrowserRouter, Route, Switch } from 'react-router-dom';
import Categories from './containers/Categories';
import Home from './containers/Home';
import NewPostForm from './containers/NewPostForm';
import OpenedPost from './containers/OpenedPost';
import UserProfile from "./containers/UserProfile";
import Categorie from "./containers/Categorie";
import Chat from "./containers/Chat";

export const HOME_PAGE_PATH = "/";
export const NEW_POST_PAGE_PATH = "/newPost";
export const OPENED_POST_PAGE_PATH = "/post/:id";
export const CATEGORIES_PAGE_PATH = "/categories";
export const USER_PROFILE_PAGE_PATH = "/user/:id";
export const CATEGORIE_PAGE_PATH = "/categorie/:title";
export const CHAT_PAGE_PATH = "/chat";

interface PropsFromState {
  path: string
}

class Routes extends Component<PropsFromState> {
  render() {
    return (
      <div>
        <BrowserRouter>
          <Switch>
            <Route exact path={HOME_PAGE_PATH} component={Home}></Route>
            <Route path={NEW_POST_PAGE_PATH} component={NewPostForm}></Route>
            <Route path={OPENED_POST_PAGE_PATH} component={OpenedPost}></Route>
            <Route path={CATEGORIES_PAGE_PATH} component={Categories}></Route>
            <Route path={USER_PROFILE_PAGE_PATH} component={UserProfile}></Route>
            <Route path={CATEGORIE_PAGE_PATH} component={Categorie}></Route>
            <Route path={CHAT_PAGE_PATH} component={Chat}></Route>
          </Switch>
        </BrowserRouter>
      </div>
    );
  }
}

const mapStateToProps = (reducer: any) => {
  return {
    path: reducer.ui.currentPagePath
  }
}

export default connect(mapStateToProps)(Routes);