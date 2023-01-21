import { Button, Card, CardContent, FormControl, InputLabel, MenuItem, OutlinedInput, Select, TextField } from '@material-ui/core';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Redirect } from 'react-router';
import { Dispatch } from 'redux';
import Header from '../components/header/Header';
import { HOME_PAGE_PATH } from '../Routes';
import { NormalizedObjects } from '../store';
import { CategorieState } from '../store/categorie/types';
import { initAddPost } from '../store/post/action';
import { InitAddPostInput } from '../store/post/types';
import { startSpinner } from '../store/ui/action';
import { UiState } from '../store/ui/types';
import { Error, UserState } from '../store/user/types';
import styles from "./css/newPostForm.module.css";

interface PropsFromState {
  ui: UiState,
  categories: NormalizedObjects<CategorieState>,
  user: UserState
}

interface PropsFromDispatch {
  initAddPost: typeof initAddPost,
  startSpinner: typeof startSpinner
}

type AllProps = PropsFromState & PropsFromDispatch;

interface IState {
  topic: number,
  content: string,
  topicError: Error,
  contentError: Error,
  redirect: boolean
}

class NewPostForm extends Component<AllProps, IState> {
  readonly state = {
    topic: 0,
    content: "",
    topicError: {
      error: false,
      errorText: ""
    },
    contentError: {
      error: false,
      errorText: ""
    },
    redirect: false
  }

  render() {
    if(this.state.redirect) 
      return <Redirect to={HOME_PAGE_PATH} />

    return (
      <div>
        <Header isLoggedUser={this.props.ui.loggedUser === 0 ? false : true}></Header>
        <div className={styles.cardContainer}>
          <Card className={styles.card}>
            <CardContent>
              <FormControl variant="outlined" fullWidth>
                <InputLabel className={styles.title}>Tema</InputLabel>
                <Select 
                  error={this.state.topicError.error}
                  value={this.state.topic}
                  onChange={this.selectTopic} 
                  input={<OutlinedInput labelWidth={50} className={styles.title}/>}>
                  {this.props.user.categories.map((categorie, index) => {
                    return(
                      <MenuItem value={categorie} key={index} className={styles.title}> 
                        {this.props.categories.byId[categorie].title}
                      </MenuItem>);
                  })}
                </Select>
              </FormControl>
              <TextField
                className={styles.title}
                id="outlined-multiline-static" rows={5}
                placeholder={"Unesite Vašu objavu ovde"}
                multiline fullWidth margin="normal" variant="outlined"
                value={this.state.content}
                onChange={this.setContent}
                error={this.state.contentError.error}
                helperText={this.state.contentError.errorText}/>
              <div className={styles.submitButton}>
                <Button variant="contained" size={"small"} style={{borderRadius: "10px"}} color={"primary"} onClick={this.submitPost}>
                  Objavi
                </Button>
              </div>
            </CardContent>
          </Card>
      </div>
    </div>
    );
  }

  selectTopic = (event: React.ChangeEvent<{ name?: string; value: any }>) => {
    this.setState({
      topic: event.target.value,
      topicError: {error: false, errorText: ""}
    });
  }

  setContent = (event: any) => {
    this.setState({
      content: event.currentTarget.value,
      contentError: {error: false, errorText: ""}
    });
  }

  submitPost = () => {
    if(this.postValidation()) {
      this.props.initAddPost({
        authorId: this.props.ui.loggedUser,
        categorie: this.state.topic,
        categorieTitle: this.props.categories.byId[this.state.topic].title,
        content: this.state.content
      });
      this.props.startSpinner();
      this.setState({redirect: true});
    }
  }

  postValidation = () => {
    let result: boolean = true;
    if(this.state.topic === 0) {
      this.setState({topicError: {error: true, errorText: "Izaberite temu!"}});
      result = false;
    }
    if(this.state.content === "") {
      this.setState({contentError: {error: true, errorText: "Unesite komentar!"}});
      result = false;
    }
    return result;
  }
}

const mapStateToProps = (rootReducer: any) => {
  return {
    ui: rootReducer.ui,
    categories: rootReducer.categories,
    user: rootReducer.users.byId[rootReducer.ui.loggedUser]
  }
}

const mapDispatchToProps = (dispatch: Dispatch) => {
  return {
    initAddPost: (post: InitAddPostInput) => dispatch(initAddPost(post)),
    startSpinner: () => dispatch(startSpinner())
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(NewPostForm);