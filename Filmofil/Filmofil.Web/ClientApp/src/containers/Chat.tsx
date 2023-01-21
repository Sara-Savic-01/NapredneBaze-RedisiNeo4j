import { Button, Card, CardContent, List, ListItem, ListItemText, Paper, TextField, Typography } from "@material-ui/core";
import React, { Component, ChangeEvent } from "react";
import { connect } from "react-redux";
import Header from "../components/header/Header";
import style from "./css/chat.module.css";
import { UiState } from "../store/ui/types";
import { NormalizedObjects } from "../store";
import { CategorieState } from "../store/categorie/types";
import { MessageState } from "../store/message/types";
import { UserState } from "../store/user/types";
import { initSendMessage, fetchMessagesAndSubscribe } from "../store/message/action";
import { Dispatch } from "redux";
import { startSpinner, selectChatCategorie } from "../store/ui/action";

interface PropsFromDispatch {
  initSendMessage: typeof initSendMessage,
  startSpinner: typeof startSpinner,
  selectChatCategorie: typeof selectChatCategorie,
  fetchMessages: typeof fetchMessagesAndSubscribe
}

interface PropsFromState {
  ui: UiState,
  categories: NormalizedObjects<CategorieState>,
  messages: NormalizedObjects<MessageState>,
  users: NormalizedObjects<UserState>
}

interface State {
  message: string,
  selected: number
}

type allProps = PropsFromDispatch & PropsFromState;

class Chat extends Component<allProps, State>{
  readonly state = {
    message: "",
    selected: 0
  }

  componentWillUpdate() {
    if(!this.props.messages.isLoaded && this.props.users.isLoaded) {
      this.props.fetchMessages();
    }
  }

  render() {
    return(
      <div>
        <Header isLoggedUser={this.props.ui.loggedUser !== 0}></Header>
        {this.renderChatPage()}
      </div>
    )
  }

  renderChatPage = () => {
    if(!this.props.categories.isLoaded || !this.props.users.isLoaded) 
      return(<div></div>);

    return(
    <div className={style.container}>
      <Paper className={style.paper}>
        <div className={style.categories}>
          <List>
            {this.props.users.byId[this.props.ui.loggedUser].categories.map(categorie => {
              return(
                <ListItem key={categorie} divider={true} button onClick={() => this.onCategorieClick(categorie)}
                  selected={categorie === this.props.ui.selectedChatCategorie}>
                  <ListItemText primary={this.props.categories.byId[categorie].title} />
                </ListItem>
              )
            })}
          </List>
        </div>
        {this.renderMessages()}
      </Paper>
    </div>)
  }

  renderMessages = () => {
    if(this.props.ui.selectedChatCategorie === 0) 
      return(
        <Card className={style.messageCard}>
          <CardContent className={style.content}>
            <Typography variant="h6">FIlmofil</Typography>
            <Typography variant="body2">Izaberite temu kojoj želite da se priključite</Typography>
          </CardContent>
        </Card>
      );

    if(this.props.messages.isLoaded) {
      return(
        <div className={style.messages}>
          <div className={style.messagesContainer}>
            <div className={style.messageContainer}>
              {this.props.categories.byId[this.props.ui.selectedChatCategorie].messages.map(message => {
                return(
                  <Card className={style.messageCard} key={message}>
                    <CardContent className={style.content}>
                      <Typography variant="h6">{this.props.users.byId[this.props.messages.byId[message].sender].username}</Typography>
                      <Typography variant="body2">{this.props.messages.byId[message].content}</Typography>
                    </CardContent>
                  </Card>
                )
              })}
            </div>
          </div>
          <div className={style.input}>
            <TextField id="outlined-full-width" label="Poruka" className={style.textField}
              style={{ margin: 8 }} placeholder="Unesite Vašu poruku ovde"
              fullWidth margin="normal" InputLabelProps={{shrink: true}}
              variant="outlined" value={this.state.message} onChange={this.onContentChange}/>
            <Button className={style.button} color="primary" 
              variant="contained" size={"small"} onClick={this.onSendButtonClick}>
              Pošaljite
            </Button>
          </div>
        </div>);
    }
  }

  onContentChange = (event: ChangeEvent<HTMLInputElement>) => {
    this.setState({message: event.currentTarget.value});
  }

  onCategorieClick = (categorie: number) => {
    this.props.selectChatCategorie(categorie);
    this.setState({...this.state, selected: categorie});
  }

  onSendButtonClick = () => {
    if(this.state.message !== "") {
      this.props.startSpinner();
      this.props.initSendMessage({
        categorie: this.props.ui.selectedChatCategorie,
        content: this.state.message,
        id: 0,
        sender: this.props.ui.loggedUser
      });
      this.setState({...this.state, message: ""});
    }

  }

}

const mapStateToProps = (reducer: any) => {
  return {
    ui: reducer.ui,
    categories: reducer.categories,
    messages: reducer.messages,
    users: reducer.users
  }
}

const mapDispatchToProps = (dispatch: Dispatch) => {
  return {
    initSendMessage: (message: MessageState) => dispatch(initSendMessage(message)),
    startSpinner: () => dispatch(startSpinner()),
    selectChatCategorie: (categorie: number) => dispatch(selectChatCategorie(categorie)),
    fetchMessages: () => dispatch(fetchMessagesAndSubscribe())
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(Chat);