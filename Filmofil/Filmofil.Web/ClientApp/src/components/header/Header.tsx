import { AppBar, Avatar,  IconButton, Toolbar, Typography } from '@material-ui/core';
import Button from "@material-ui/core/Button";
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Redirect } from 'react-router';
import { Dispatch } from 'redux';
import logoutIcon from "../../assets/logoutfin.jpg";
import filmiIcon from "../../assets/filmi_icon.png";
import userProfile from "../../assets/user.png";
import { CATEGORIES_PAGE_PATH, NEW_POST_PAGE_PATH } from '../../Routes';
import { LOGGED_USER_KEY, removeItemFromLocalStorage } from '../../services/local-storage';
import { NormalizedObjects } from '../../store';
import { logoutUser, openLoginDialog, openSignupDialog, startSpinner } from '../../store/ui/action';
import { UiState } from '../../store/ui/types';
import { fetchUserData } from '../../store/user/action';
import { UserState } from '../../store/user/types';
import Error from "../error/Error";
import Login from '../login/Login';
import SignUp from '../signup/SignUp';
import styles from "./css/header.module.css";
import {  fetchMessagesAndSubscribe } from '../../store/message/action';

interface IProps {
  isLoggedUser: boolean
}

interface PropsFromState {
  ui: UiState,
  users: NormalizedObjects<UserState>
}

interface propsFromDispatch {
  openLoginDialog: typeof openLoginDialog,
  openSignupDialog: typeof openSignupDialog,
  logoutUser: typeof logoutUser,
  fetchUserData: typeof fetchUserData,
  startSpinner: typeof startSpinner,
  fetchMessagesAndSubscribe: typeof fetchMessagesAndSubscribe
}

interface IState {
  redirect: boolean,
  path: string
}

type allProps = IProps & propsFromDispatch & PropsFromState;

class Header extends Component<allProps, IState> {
  readonly state = {
      path: "",
      redirect: false
  }

  render() {
    if (this.state.redirect && window.location.pathname !== this.state.path) {
      return <Redirect push to={this.state.path} />
    }
    
    return (
      <div>
        <AppBar position="static">
                <Toolbar className={styles.toolbar}>
            <div className={styles.filmiLogo}>
              <div onClick={this.goHome}><Avatar className={styles.filmiIcon} src={filmiIcon} ></Avatar></div>
              <div onClick={this.goHome}><Typography className={styles.filmiTypography} variant="h6" color="inherit" noWrap>Filmofil</Typography></div>
            </div>
            <div className={styles.topCenterButtonsContainer}>
                <Button 
                  className={styles.categoriesButton} 
                  variant={"outlined"} color={"inherit"}
                            onClick={this.showAllCategories}>
                  Sve kategorije
                </Button>
                <Button className={styles.categoriesButton}
                  variant={"outlined"} color={"inherit"} 
                  onClick={this.onNewPostClick}  >
                  Dodajte objavu
                </Button>
                <Button 
                  className={styles.categoriesButton} 
                  variant={"outlined"} color={"inherit"}
                  onClick={this.openChat}>
                  Ćaskanje
                </Button>
            </div>
            <div className={this.props.isLoggedUser ? styles.hidden : styles.signUp}>
              <Button onClick={this.props.openSignupDialog} className={styles.button} color="inherit" variant="outlined">Registrujte se</Button>
                        <Button onClick={this.props.openLoginDialog} className={styles.button} color="inherit" variant="outlined" >Prijavite se</Button>
            </div>
            <div className={this.props.isLoggedUser ? styles.accountMenu : styles.hidden}>
              <IconButton onClick={this.logout}>
                <Avatar className={styles.accountIcon} src={logoutIcon}></Avatar>
              </IconButton>
              <IconButton onClick={this.onUserProfileClick}>
                <Avatar className={styles.accountIcon} src={userProfile}></Avatar>
              </IconButton>
            </div>
          </Toolbar>
        </AppBar>
        <Login></Login>
        <SignUp></SignUp>
        <Error></Error>
        {/* <div hidden={!this.props.ui.spinner}><CircularProgress className={styles.spinner2} /></div> */}
      </div>
    );
  }

  logout = () => {
    removeItemFromLocalStorage(LOGGED_USER_KEY);
    this.props.logoutUser();
    this.setState({...this.state, redirect: true, path: "/"});
  }

  onNewPostClick = () => {
    this.props.isLoggedUser ? 
      this.setState({ redirect: true, path: NEW_POST_PAGE_PATH}) : 
      this.props.openLoginDialog();
  }

  showAllCategories = () => {
    this.props.isLoggedUser ? 
      this.setState({redirect: true, path: CATEGORIES_PAGE_PATH}) :
      this.props.openLoginDialog();
  }

  onUserProfileClick = () => {
    this.props.startSpinner();
    this.setState({redirect : true, path: "/user/" + this.props.ui.loggedUser});
  }

  goHome = () => {
    this.setState({...this.state, redirect: true, path: "/"});
  }

  openChat = () => {
    if(this.props.ui.loggedUser !== 0) {
      this.setState({path: "/chat", redirect: true});
      this.props.fetchMessagesAndSubscribe();
    }
    else this.props.openLoginDialog();
  }
}

const mapDispatchToProps = (dispatch: Dispatch) => {
  return {
    openLoginDialog: () => dispatch(openLoginDialog()),
    openSignupDialog: () => dispatch(openSignupDialog()),
    logoutUser: () => dispatch(logoutUser()),
    fetchUserData: () => dispatch(fetchUserData()),
    startSpinner: () => dispatch(startSpinner()),
    fetchMessagesAndSubscribe: () => dispatch(fetchMessagesAndSubscribe())
  }
}

const mapStateToProps = (reducer: any) => {
  return {
    ui: reducer.ui,
    users: reducer.users
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(Header);