import {  Button, Grid, List, ListItem,  ListItemSecondaryAction, ListItemText, Typography } from "@material-ui/core";
import React, { Component } from "react";
import { connect } from "react-redux";
import { Redirect } from "react-router";
import { Dispatch } from "redux";
import Header from "../components/header/Header";
import { NormalizedObjects } from "../store";
import { initJoinCategorie, initLeaveCategorie } from "../store/categorie/action";
import { CategorieState } from "../store/categorie/types";
import { fetchCategoriePosts } from "../store/post/action";
import { startSpinner } from "../store/ui/action";
import { UiState } from "../store/ui/types";
import { UserState } from "../store/user/types";
import styles from "./css/categories.module.css";

export const POPULAR_POSTS_CATEGORY = "Popular";
export const NEW_POSTS_CATEGORY = "New";
export const CONTROVERSIAL_POSTS_CATEGORY = "Controversial";

interface PropsFromState {
  ui: UiState,
  users: NormalizedObjects<UserState>,
  categories: NormalizedObjects<CategorieState>
}

interface PropsFromDispatch {
  fetchCategoriePosts: typeof fetchCategoriePosts,
  initJoinCategorie: typeof initJoinCategorie,
  initLeaveCategorie: typeof initLeaveCategorie,
  startSpinner: typeof startSpinner
}

interface IState {
  redirect: boolean,
  path: string
}

type allProps = PropsFromState & PropsFromDispatch;

class Categories extends Component<allProps, IState> {
  readonly state = {
    redirect: false,
    path: ""
  }

  render() {
    if (this.state.redirect && window.location.pathname !== this.state.path)
        return <Redirect push to={this.state.path} />

    return (
      <div>
        <Header isLoggedUser = {this.props.ui.loggedUser === 0 ? false : true}></Header>
        <div className={styles.categoriesContainer}>
          <div className={styles.categories}>
          <Grid container spacing={2}>
            <Grid item xs>
            <Typography variant="h6" className={styles.title}>
                Žanrovi
            </Typography>

              {this.props.categories.allIds.map((categorie, index) =>
                (<List key={index}>
                  <ListItem button onClick={() => this.openCategorie(this.props.categories.byId[categorie].title)}>
                    <ListItemText primary={this.props.categories.byId[categorie].title}/>
                    <ListItemSecondaryAction>
                      <Button 
                        onClick={() => this.leaveOrJoinCategorieButtonClick(categorie)}
                        color={this.alreadyJoined(categorie) ? "secondary" : "primary"}
                        size={"small"}
                        style={{borderRadius: "10px"}}
                        variant="contained">
                          {this.alreadyJoined(categorie) ? "Izađi" : "Priključi se"}
                      </Button>
                    </ListItemSecondaryAction>
                  </ListItem>
                </List>))
              }
            </Grid>
          </Grid>
        </div>
      </div>
      </div>
    );
  }

  alreadyJoined = (categorie: number) : boolean => {
    return this.props.users.byId[this.props.ui.loggedUser].categories.includes(categorie);
  }

  leaveOrJoinCategorieButtonClick = (categorie: number) => {
    this.props.startSpinner();
    if(this.props.users.byId[this.props.ui.loggedUser].categories.includes(categorie)) {
      this.props.initLeaveCategorie(categorie, this.props.ui.loggedUser);
    }
    else {
      this.props.initJoinCategorie(categorie, this.props.ui.loggedUser);
    }
  }

  openCategorie = (categorie: string) => {
    this.props.startSpinner();
    this.props.fetchCategoriePosts(categorie, POPULAR_POSTS_CATEGORY);
    this.setState({redirect: true, path: "/categorie/" + categorie});
  }
}

const mapStateToProps = (rootReducer: any) => {
  return {
    ui: rootReducer.ui,
    users: rootReducer.users,
    categories: rootReducer.categories
  }
}

const mapStateToDispatch = (dispatch : Dispatch) => {
  return {
    fetchCategoriePosts: (categorie: string) => dispatch(fetchCategoriePosts(categorie, POPULAR_POSTS_CATEGORY)),
    initJoinCategorie: (categorie: number, user: number) => dispatch(initJoinCategorie(categorie, user)),
    initLeaveCategorie: (categorie: number, user: number) => dispatch(initLeaveCategorie(categorie, user)),
    startSpinner: () => dispatch(startSpinner())
  }
}

export default connect(mapStateToProps, mapStateToDispatch)(Categories);