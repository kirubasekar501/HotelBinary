export class NodeTree {
  data?: string;
  right?: NodeTree;
  left?: NodeTree;
}

export class BinaryEvaluationResponse {
  NodeTree?: Array<any>;
  result?: number;

}

export class nptr {
  left?: any;
  right?: any;
  data?: any;
  constructor(c: any) {
    this.left = null;
    this.right = null;
    this.data = c;
  }
}
