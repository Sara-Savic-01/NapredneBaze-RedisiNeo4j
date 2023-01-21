import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField } from '@material-ui/core';
import React, { ChangeEvent, Component } from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { validateEmail, validatePassword, validateUsername } from '../../services/auth';
import { NormalizedObjects } from '../../store';
import { closeSignupDialog, startSpinner } from '../../store/ui/action';
import { UiState } from '../../store/ui/types';
import { initSignUp } from '../../store/user/action';
import { Error, InitSignUpData, UserState } from '../../store/user/types';
import style from "./css/signUp.module.css";

interface propsFromState {
  users: NormalizedObjects<UserState>,
  ui: UiState
}

interface propsFromDispatch {
  initSignUp: typeof initSignUp,
  closeDialog: typeof closeSignupDialog,
  startSpinner: typeof startSpinner
}

interface IState {
  email: string,
  password: string,
  username: string,
  emailError: Error,
  usernameError: Error,
  passwordError: Error
}

type allProps = propsFromState & propsFromDispatch;

class SignUp extends Component<allProps, IState> {
  readonly state = {
    email: "",
    password: "",
    username: "",
    emailError: {error: false, errorText: ""},
    usernameError: { error: false, errorText: "" },
    passwordError: { error: false, errorText: "" }
  }

  componentWillReceiveProps() {
    this.setState({
      emailError: { error: false, errorText: "" },
      usernameError: { error: false, errorText: "" },
      passwordError: { error: false, errorText: "" }
    })
  }

  render() {
    return (
      <div>
        <Dialog open={this.props.ui.isSignupDialogOpened}>
          <DialogTitle>Registrujte se</DialogTitle>
          <DialogContent className={style.dialogContent}>
            <TextField 
              helperText={this.state.emailError.errorText} error={this.state.emailError.error} 
              onChange={this.onEmailChange} value={this.state.email} 
              margin="dense" label="Email adresa" fullWidth />
            <TextField 
              helperText={this.state.usernameError.errorText} error={this.state.usernameError.error} 
              onChange={this.onUsernameChange} value={this.state.username} 
              margin="dense" label="Korisničko ime" fullWidth />
            <TextField 
              helperText={this.state.passwordError.errorText} error={this.state.passwordError.error}
              onChange={this.onPasswordChange} value={this.state.password}
              margin="dense" label="Šifra" fullWidth />
          </DialogContent>
          <DialogActions>
            {/* <div hidden={!this.props.ui.spinner}>
              <CircularProgress className={style.spinner} color="secondary"></CircularProgress>
            </div> */}
                    <div hidden={this.props.ui.spinner}><Button variant="contained" size={"small"} color="primary" onClick={this.onSubmitClick} >Registrujte se</Button></div>
            <Button 
              onClick={this.props.closeDialog} variant="contained" size={"small"} color="primary">Otkažite</Button>
          </DialogActions>
        </Dialog>
      </div>
    );
  }

  onEmailChange = (event: ChangeEvent<HTMLInputElement>) => {
    this.setState({email: event.currentTarget.value});
  }

  onPasswordChange = (event: ChangeEvent<HTMLInputElement>) => {
    this.setState({password: event.currentTarget.value});
  }

  onUsernameChange = (event: ChangeEvent<HTMLInputElement>) => {
    this.setState({ username: event.currentTarget.value });
  }

  onSubmitClick = () => {
    if(this.checkCredentials()) {
      this.props.startSpinner();
      this.props.initSignUp({
        username: this.state.username,
        email: this.state.email,
        password: this.state.password
      });
    }
  }

  checkCredentials = (): boolean => {
    let email = this.checkEmail();
    let password = this.checkPassword();
    let username = this.checkUsername();
    return email && password && username;
  }

  checkEmail = () => {
    const result = validateEmail(this.state.email);
    this.setState({
      emailError: {
        error: !result,
        errorText: result ? "" : "Nevalidna email adresa"
      }
    });
    return result;
  }

  checkPassword = () => {
    const result = validatePassword(this.state.password);
    this.setState({
      passwordError: {
        error: !result,
        errorText: result ? "" : "Nevalida šifra (min 6 karaktera)"
      }
    });
    return result;
  }

  checkUsername = () => {
    const result = validateUsername(this.state.username);
    this.setState({
      usernameError: {
        error: !result,
        errorText: result ? "" : "Nevalido korisničko ime (min 6 karaktera)"
      }
    });
    return result;
  }
}

const mapStateToProps = (rootReducer: any) => {
  return {
    users: rootReducer.users,
    ui: rootReducer.ui
  }
}

const mapDispatchToProps = (dispatch: Dispatch) => {
  return {
    initSignUp: (userData: InitSignUpData) => dispatch(initSignUp(userData)),
    closeDialog: () => dispatch(closeSignupDialog()),
    startSpinner: () => dispatch(startSpinner())
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(SignUp);