import {  Button, Grid, List, ListItem,  ListItemSecondaryAction, ListItemText, Paper, Typography } from '@material-ui/core';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import Header from "../components/header/Header";
import Post from '../components/post/Post';
import { NormalizedObjects } from '../store';
import { initLeaveCategorie } from '../store/categorie/action';
import { PostState } from '../store/post/types';
import { UiState } from '../store/ui/types';
import { UserState } from '../store/user/types';
import styles from "./css/userProfile.module.css";
import { fetchUserData } from '../store/user/action';
import { RouteComponentProps } from 'react-router';
import { CategorieState } from '../store/categorie/types';

interface propsFromState {
  ui: UiState,
  posts: NormalizedObjects<PostState>,
  users: NormalizedObjects<UserState>,
  categories: NormalizedObjects<CategorieState>
}

interface propsFromDispatch {
  initLeaveCategorie: typeof initLeaveCategorie,
  fetchUserData: typeof fetchUserData
}

type allProps = propsFromState & propsFromDispatch & RouteComponentProps<{ id: string }>;


class UserProfile extends Component<allProps> {

  componentDidMount() {
    this.props.fetchUserData();
  }

  render() {
    const user : UserState = this.props.users.byId[this.props.ui.loggedUser];
    
    return(
      <div>
        <Header isLoggedUser={this.props.ui.loggedUser !== 0}></Header>
        {this.renderUserPage(user)}
      </div>
    )
  }

  renderUserPage = (user: UserState) => {
    if(!this.props.users.isLoaded || this.props.ui.spinner || 
      !this.props.posts.isLoaded || this.props.ui.loggedUser === 0)
      return (<div></div>);

    return(
      <div className={styles.container}>
        <div className={styles.postsContainer}>
          {this.renderPosts(user)}
        </div>
        <div className={styles.categoriesContainer}>
          <div className={styles.categories}>
            <Paper elevation={3}>
            <Grid container spacing={2} className={styles.grid}>
                <Grid item xs>
                <Typography variant="h6" className={styles.title}>
                    {" Žanrovi korisnika "+ user.username}
                </Typography>
                  {user.categories.length === 0 ? "Pronađite sve žanrove kojima želite da se pridružite" : ""}
                  {user.categories.map((categorie, index) =>
                    (<List key={index}>
                      <ListItem>
                        
                        <ListItemText primary={this.props.categories.byId[categorie].title}/>
                        <ListItemSecondaryAction>
                          <Button 
                            onClick={() => this.props.initLeaveCategorie(categorie, user.id)}
                            color={"secondary"}
                            size={"small"}
                            style={{borderRadius: "10px"}}
                            variant="contained">
                              Izađi
                          </Button>
                        </ListItemSecondaryAction>
                      </ListItem>
                    </List>))
                  }
                </Grid>
              </Grid>
            </Paper>                 
            </div>
           </div>  
        </div>);
  }

  renderPosts = (user: UserState) => {
    if(user.posts.length === 0) {
      return(
        <Typography className={styles.noPosts} variant="h6">Još ništa niste objavili!</Typography>
      );
    }
    else {
      return user.posts.map(post => {
        return (
          <Post key={post}
            isOpened={this.props.ui.isOpenedSinglePost}
            postState={this.props.posts.byId[post]}
            cardWidthInPercentage="90%">
          </Post>
        )
      })
    }
  }
}

const mapPropsToState = (reducer: any) => {
  return {
    ui: reducer.ui,
    posts: reducer.posts,
    users: reducer.users,
    categories: reducer.categories
  }
}

const mapPropsToDispatch = (dispatch: Dispatch) => {
  return {
    initLeaveCategorie: (categorie: number, user: number) => dispatch(initLeaveCategorie(categorie, user)),
    fetchUserData: () => dispatch(fetchUserData())
  }
}

export default connect(mapPropsToState, mapPropsToDispatch)(UserProfile);