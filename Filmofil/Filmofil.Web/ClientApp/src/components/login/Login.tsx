import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField } from '@material-ui/core';
import React, { ChangeEvent, Component } from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { NormalizedObjects } from '../../store';
import { closeLoginDialog, setLoggedUser, startSpinner } from '../../store/ui/action';
import { login } from '../../store/user/action';
import { Error, UserState } from '../../store/user/types';
import { Redirect } from 'react-router';

interface IState {
  username: string,
  password: string,
  emailError: Error,
  passwordError: Error,
  path: string,
  redirect: boolean
}

interface propsFromState {
  users: NormalizedObjects<UserState>,
  isOpen: boolean
}

interface propsFromDispatch {
  setLoggedUser: typeof setLoggedUser,
  closeDialog: typeof closeLoginDialog,
  login: typeof login,
  startSpinner: typeof startSpinner
}

type allProps = propsFromDispatch & propsFromState;

class Login extends Component<allProps, IState> {
  readonly state = {
    username: "",
    password: "",
    emailError: { error: false, errorText: "" },
    passwordError: { error: false, errorText: "" },
    path: "",
    redirect: false
  }

  render() {
    if (this.state.redirect && window.location.pathname !== this.state.path) {
      return <Redirect push to={this.state.path} />
    }

    return (
      <div>
        <Dialog open={this.props.isOpen}>
          <DialogTitle>Prijavite se</DialogTitle>
          <DialogContent>
            <TextField 
              helperText={this.state.emailError.errorText} error={this.state.emailError.error}
              onChange={this.onUsernameChange} value={this.state.username}
              margin="dense" label="Korisničko ime" type="username" fullWidth />
            <TextField 
              helperText={this.state.passwordError.errorText} error={this.state.passwordError.error}
              onChange={this.onPasswordChange} value={this.state.password} 
              margin="dense" label="Šifra" type="password" fullWidth />
          </DialogContent>
          <DialogActions>
            <Button onClick={this.onSubmitClick} variant="contained" size={"small"} color="primary">Prijavite se</Button>
            <Button onClick={this.props.closeDialog} variant="contained" size={"small"} color="primary">Otkažite</Button>
          </DialogActions>
        </Dialog>
      </div>
    );
  }

  onUsernameChange = (event: ChangeEvent<HTMLInputElement>) => {
    this.setState({ username: event.currentTarget.value });
  }

  onPasswordChange = (event: ChangeEvent<HTMLInputElement>) => {
    this.setState({ password: event.currentTarget.value });
  }


  onSubmitClick = () => {
    if(this.state.username !== "" && this.state.password !== "") {
      this.props.login(this.state.username, this.state.password);
      this.setState({...this.state, path: "/", redirect: true});
      this.props.startSpinner();
    }
  }
}

const mapDispatchToProps = (dispatch: Dispatch) => {
  return {
    setLoggedUser: (userId: number) => dispatch(setLoggedUser(userId)),
    closeDialog: () => dispatch(closeLoginDialog()),
    login: (username: string, password: string) => dispatch(login(username, password)),
    startSpinner: () => dispatch(startSpinner())
  }
}

const mapStateToProps = (rootReducer: any) => {
  return {
    users: rootReducer.users,
    isOpen: rootReducer.ui.isLoginDialogOpened
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(Login);