

class CommentBox extends React.Component {
    constructor(props) {
        super(props);
        this.state = { data: [] };
    }
    render() {
        return (
           
            <div className="commentBox">
                <div> {this.state.data} </div>
                <div> {this.props.url} </div>
                <h1>Comments</h1>
                <CommentList data={this.state.data} />
                <CommentForm />
            </div>
        );
    }
}
class CommentList extends React.Component {
    render() {
                 const commentNodes = this.props.data.map(comment => (
            <Comment author={comment.Author} key={comment.Id}>
                {comment.Text}
            </Comment>
        ));
        return <div className="commentList">{commentNodes}</div>;
        
    }
}

class CommentForm extends React.Component {
    render() {
        return (
            <div className="commentForm">Hello, world! I am a CommentForm.</div>
        );
    }
}
class Comment extends React.Component {
    render() {
        return (
            <div className="comment">
                <h2 className="commentAuthor">{this.props.author}</h2>
                {this.props.children}
            </div>
        );
    }
}
           
ReactDOM.render(
    <CommentBox url="/Comments" />,
    document.getElementById('content')
);