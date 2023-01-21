import { Typography } from '@material-ui/core';
import React, { Component } from 'react';

interface IProps {
  content: string
}

class PostContent extends Component<IProps> {
  render() {
    return (
      <div>
        <Typography align="left" variant="h6">{this.props.content}</Typography>
      </div>
    );
  }
}

export default PostContent;