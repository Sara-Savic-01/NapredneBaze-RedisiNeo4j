import ReactDOM from 'react-dom';
import './index.css';
import * as serviceWorker from './serviceWorker';
import configureStore from './store';
import { Provider } from 'react-redux';
import App from './App';
import React, { Component } from 'react';
import { fetchData } from './store/app/action';
import { startSpinner } from './store/ui/action';

export const store = configureStore();

class Root extends Component {
  componentDidMount() {
    store.dispatch(startSpinner());
    store.dispatch(fetchData());
   
  }

  render() {
    return (
      <Provider store={store}>
        <App />
      </Provider>
    )
  }
}

ReactDOM.render(<Root />, document.getElementById('root'));
serviceWorker.unregister();
