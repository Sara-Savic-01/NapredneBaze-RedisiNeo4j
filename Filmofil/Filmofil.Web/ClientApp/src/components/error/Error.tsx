import { Button, Snackbar } from '@material-ui/core';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { closeErrorDialog } from '../../store/ui/action';
import { UiState } from '../../store/ui/types';

interface PropsFromState {
  ui: UiState
}

interface PropsFromDispatch {
  closeDialog: typeof closeErrorDialog
}

type allProps = PropsFromState & PropsFromDispatch;

class Error extends Component<allProps> {
  render() {
    return (
      <div>
      <Snackbar
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'left',
        }}
        open={this.props.ui.error}
        message={this.props.ui.errorMessage}
        action={
          <React.Fragment>
            <Button color="primary" size="medium" onClick={this.props.closeDialog}>
              Zatvorite
            </Button>
          </React.Fragment>
        }
      />
      </div>
    )
  }
}

const mapStateToProps = (reducer: any) => {
  return {
    ui: reducer.ui
  }
}

const mapDispatchToProps = (dispatch: Dispatch) => {
  return {
    closeDialog: () => (dispatch(closeErrorDialog()))
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(Error);