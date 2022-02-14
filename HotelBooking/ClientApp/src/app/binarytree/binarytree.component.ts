import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BinaryEvaluationResponse, nptr } from './binarytree.model';

@Component({
  selector: 'app-binarytree',
  templateUrl: './binarytree.component.html',
})
export class BinarytreeComponent implements OnInit {

  /*public binaryTreeResults: BinaryEvaluationResponse = new BinaryEvaluationResponse();*/

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  public root: any = [];
  public rootArray: any = [];
  public preOrderRepresentation: string = "";
  public postOrderRepresentation: string = "";
  public inOrderRepresentation: string = "";
  public result: string = "";

  ngOnInit(): void {
  }
  evaluate() {
    this.preOrderRepresentation = "";
    this.inOrderRepresentation = "";
    this.postOrderRepresentation = "";
    this.result = "";
    var inputString = (<HTMLInputElement>document.getElementById("inputExpression")).value;
   // let s = "((15/(7-(1+1)))*-3)-(2+(1+1))"
    inputString = "(" + inputString;
    inputString += ")";
    this.root = this.buildTree(inputString);
    this.preorder(this.root);
    console.log(this.root);
    this.result = this.evaluateTree(this.root).toString();
  }

  preorder(root: any) {
    if (root != null) {
      //this.rootArray.push(root);
      //console.log(root.data);
      this.preOrderRepresentation = this.preOrderRepresentation + root.data + ", ";
      this.preorder(root.left);
      this.inOrderRepresentation = this.inOrderRepresentation + root.data + ", ";
      this.preorder(root.right);
      this.postOrderRepresentation = this.postOrderRepresentation + root.data + ", ";

    }
  }

  evaluateTree(root: any) {
    if (root == null)
      return 0;
    if (root.left == null && root.right == null)
      return Number(root.data);
    var leftEval: any = this.evaluateTree(root.left);
    var rightEval: any = this.evaluateTree(root.right);
    if (root.data === ("+"))
      return (Number(leftEval) + Number(rightEval));
    if (root.data === ("-"))
      return (Number(leftEval) - Number(rightEval));
    if (root.data === ("*"))
      return (Number(leftEval) * Number(rightEval));
    if (root.data === ("/"))
      return (Number(leftEval) / Number(rightEval));
    return (Number(leftEval) / Number(rightEval));
  }

  newNode(c: any) {
    let n = new nptr(c);
    return n;
  }

  buildTree(s: any) {
    let stN: any[] = [];
    let stC = [];
    let t, t1, t2;
    let p = new Array(123);
    p['+'.charCodeAt(0)] = p['-'.charCodeAt(0)] = 1;
    p['/'.charCodeAt(0)] = p['*'.charCodeAt(0)] = 2;
    p['^'.charCodeAt(0)] = 3;
    p[')'.charCodeAt(0)] = 0;
    let _number: string = "";
    let negativeNumberFound: Boolean = false;
    let doNotPop: Boolean = false;
    for (let i = 0; i < s.length; i++) {
      if (s[i] == '-') {
        if (!isNaN(s[i + 1])) {
          negativeNumberFound = true;
        }
      }
      if (s[i] == '(') {
        stC.push(s[i]);
      }
      else if ((/[0-9]/).test(s[i])) {
        _number = _number.concat(s[i]);
      }
      else if (p[s[i].charCodeAt()] > 0) {
        let handleNegative: Boolean = false;
        let processNegativeValues: Boolean = false;
        if (_number != "") {
          var currentChar = s[i];
          var previousNumber = _number;
          let indexToTrack = i - (s[i].length + _number.length + 1);
          if (indexToTrack >= 0) {
            var charPreviousToNumber = s[indexToTrack];
            if (isNaN(charPreviousToNumber)) {
              processNegativeValues = true;
            }
          }
          while (negativeNumberFound == true && stC.length > 1 && processNegativeValues == true && stC[stC.length - 1] == '-') {
            handleNegative = true;
            t = stN[stN.length - 1];
            t2 = t.right;
            t.left = null;
            t.right = null;

            if (stN.length > 0) {
              t1 = this.newNode(stC[stC.length - 1]);
              stC.pop();
            }
            else {
              t1 = null;
            }
            stN.pop();
            t.left = t2;
            t.right = t1;
            stN.push(t);

            t = this.newNode(_number);
            stN.push(t);
            _number = "";

            if (stC.length != 0 && stC[stC.length - 1] == '(') {
              if (stN.length > 0) {
                t1 = stN[stN.length - 1];
                stN.pop();
              }
              else {
                t1 = null;
              }

              t = stN[stN.length - 1];
              t2 = t.left;
              t.left = null;

              t.left = t2;
              t.right.right = t1;
              stN.push(t);
              stC.pop();
              doNotPop = true;

            }
            negativeNumberFound = false;
          }
          if (handleNegative == false) {
            t = this.newNode(_number);
            stN.push(t);
            _number = "";
          }
        }

        var chartoCheck: any = stC[stC.length - 1];
        if (i == 17) {
          chartoCheck = '*';
        }
        while (stC.length != 0 && stC[stC.length - 1] != '(' && p[stC[stC.length - 1].charCodeAt()] >= p[s[i].charCodeAt()]) {
          t = this.newNode(stC[stC.length - 1]);
          stC.pop();
          t1 = stN[stN.length - 1];
          stN.pop();
          t2 = stN[stN.length - 1];
          stN.pop();
          t.left = t2;
          t.right = t1;
          stN.push(t);
        }
        stC.push(s[i]);
        _number = "";
      }
      else if (s[i] == ')') {
        let handleNegative: Boolean = false;
        let processNegativeValues: Boolean = false;

        if (_number != "") {
          var currentChar = s[i];
          var previousNumber = _number;
          let indexToTrack = i - (s[i].length + _number.length + 1);
          if (indexToTrack >= 0) {
            var charPreviousToNumber = s[indexToTrack];
            if (isNaN(charPreviousToNumber)) {
              processNegativeValues = true;
            }
          }
          while (negativeNumberFound == true && stC.length > 1 && processNegativeValues == true && stC[stC.length - 1] == '-') {
            handleNegative = true;
            t = stN[stN.length - 1];
            t2 = t.right;
            t.left = null;
            t.right = null;

            if (stN.length > 0) {
              t1 = this.newNode(stC[stC.length - 1]);
              stC.pop();
            }
            else {
              t1 = null;
            }
            stN.pop();
            t.left = t2;
            t.right = t1;
            stN.push(t);

            t = this.newNode(_number);
            stN.push(t);
            _number = "";

            if (stC.length != 0 && stC[stC.length - 1] == '(') {
              if (stN.length > 0) {
                t1 = stN[stN.length - 1];
                stN.pop();
              }
              else {
                t1 = null;
              }

              t = stN[stN.length - 1];
              t2 = t.left;
              t.left = null;

              t.left = t2;
              t.right.right = t1;
              stN.push(t);
              stC.pop();
              doNotPop = true;

            }
            negativeNumberFound = false;
          }
          if (handleNegative == false) {
            t = this.newNode(_number);
            stN.push(t);
            _number = "";
          }
        }
        while (stC.length != 0 &&
          stC[stC.length - 1] != '(') {
          t = this.newNode(stC[stC.length - 1]);
          stC.pop();
          t1 = stN[stN.length - 1];
          stN.pop();
          t2 = stN[stN.length - 1];
          stN.pop();
          t.left = t2;
          t.right = t1;
          stN.push(t);
          _number = "";
        }
        if (stC.length != 0 && doNotPop == false) {
          stC.pop();

        }
        else {
          doNotPop = false;
        }
      }
    }
    t = stN[stN.length - 1];
    return t;
  }


  //binaryTreeEvaluation2() {

  //  this.http.get<any>(this.baseUrl + 'weatherforecast/binary-evaluation').subscribe(result => {
  //    if (result != null) {
  //      console.log(result);
  //    }
  //  }, error => console.error(error));
  //}

  //async binaryTreeEvaluation(equation?: string) {
  //  await this.http.get<any>(this.baseUrl + 'weatherforecast/binary-evaluation').toPromise()
  //    .catch((error: any) => console.log("Update Failed"))
  //    .then((result: any) => {
  //      console.log(result);
  //      this.binaryTreeEvaluation2();
  //      this.binaryTreeResults = result;
  //    });
  //}
}
