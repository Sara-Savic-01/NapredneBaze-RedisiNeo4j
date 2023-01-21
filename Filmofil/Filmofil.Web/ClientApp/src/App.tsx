import React, { Component } from 'react';
import Routes from './Routes';
import { connect } from 'react-redux';
import styles from "./components/header/css/header.module.css";
import { CircularProgress } from '@material-ui/core';

interface PropsFromState {
  spinner: boolean
}

class App extends Component<PropsFromState> {
  render() {
    return (
      <div className="App" style={{pointerEvents: this.props.spinner ? "none" : "all"}}>
        <Routes></Routes>
        <div hidden={!this.props.spinner}><CircularProgress className={styles.spinner2} /></div>
      </div>
    );
  }
}

const mapStateToProps = (reducer: any) => {
  return {
    spinner: reducer.ui.spinner
  }
}

export default connect(mapStateToProps)(App);
