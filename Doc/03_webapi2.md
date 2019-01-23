# Dawn
2018/12/04
## 目標
WebAPI錯誤處理

## 專案初始
建置專案
```shell
$ git clone https://github.com/hyflamewow/Dawn.git
Dawn$ git checkout -b lab webapi
Moon$ npm i
Sun$ dotnet restore
```
## 除錯模式
```shell
Sun$ dotnet watch run
Moon$ npm start
```
瀏覽 http://localhost:4200/

**程式不用關閉, 直接修改程式**
## 修改Moon專案
### 建立message component 及 message
```shell
Moon$ ng g c message
Moon$ ng g s message
```
### src\app\app.component.html
```html
<router-outlet></router-outlet>
<app-message></app-message>
```
### src\app\message.service.ts
```ts
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  messages: string[] = [];
  constructor() { }
  add(message: string) {
    this.messages.push(message);
  }
  clear() {
    this.messages = [];
  }
}
```
### src\app\message\message.component.ts
```ts
import { MessageService } from '../message.service';
...
constructor(public messageService: MessageService) { }
```
### src\app\message\message.component.html
```html
<div *ngIf="messageService.messages.length">
  <h2>Messages</h2>
  <button class="clear"
          (click)="messageService.clear()">clear</button>
  <div *ngFor='let message of messageService.messages'> {{message}} </div>
</div>
```
### src\app\values\values.service.ts
```ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MessageService } from '../message.service';

@Injectable({
  providedIn: 'root'
})
export class ValuesService {

  constructor(private http: HttpClient, private messageService: MessageService) { }
  getList(): Observable<string[]> { // #200
    return this.http.get<string[]>('api/values')
      .pipe(
        catchError(this.handleError('getList', []))
      );
  }
  getNotFound(): Observable<{}> { // #404
    return this.http.get<{}>('values')
      .pipe(
        catchError(this.handleError('getNotExists', {}))
      );
  }
  getBadRequest(): Observable<{}> { // #400
    return this.http.get<{}>('api/values/error')
      .pipe(
        catchError(this.handleError('getBadRequest', {}))
      );
  }
  getNotExists(): Observable<{}> { // #200 Error
    return this.http.get<{}>('api/value')
      .pipe(
        catchError(this.handleError('getNotExists', {}))
      );
  }
  getException(): Observable<{}> {
    return this.http.get<{}>('api/values/exception')
      .pipe(
        catchError(this.handleError('getException', {}))
      );
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      switch (error.status) {
        case 200: // #URL錯了
          this.log('200服務不存在');
          break;
        // case 204: // #NoContent
        //   break;
        case 400: // #BadRequest
          this.log('400服務不存在');
          break;
        case 404: // #NotFound
          this.log('404服務不存在');
          break;
        case 500: // #Excpetion
          this.log('500服務發生未知錯誤');
          break;
        case 504: // #Server沒啟動
          this.log('504伺服器無回應');
          break;
        default: // #其他
          this.log(`${operation} failed: ${error.message}`);
          break;
      }
      return of(result as T);
    };
  }
  private log(message: string) {
    this.messageService.add(`ValuesService: ${message}`);
  }
}
```
### src\app\values.component.ts
```ts
import { Component, OnInit } from '@angular/core';
import { ValuesService } from './values.service';

@Component({
  selector: 'app-values',
  templateUrl: './values.component.html',
  styleUrls: ['./values.component.scss']
})
export class ValuesComponent implements OnInit {

  list: string[];
  constructor(private valuesService: ValuesService) { }

  ngOnInit() {
    this.getList();
  }
  getList() {
    this.valuesService.getList()
      .subscribe(list => {
        this.list = list;
      });
  }
  getNotFound() {
    this.valuesService.getNotFound()
      .subscribe();
  }
  getBadRequest() {
    this.valuesService.getBadRequest()
      .subscribe();
  }
  getNotExists() {
    this.valuesService.getNotExists()
      .subscribe();
  }
  getException() {
    this.valuesService.getException()
      .subscribe();
  }
}
```
### src\app\values\values.component.html
```html
...
<button (click)="getNotFound()">NotFound</button><br>
<button (click)="getBadRequest()">BadRequest</button><br>
<button (click)="getNotExists()">不存在</button><br>
<button (click)="getException()">例外</button>
```
## 修改Sun專案
ValuesController.cs
```cs
...
[HttpGet("Exception")]
public IActionResult GetException()
{
    throw new Exception("GetException");
}
```
## 測試
1. 關閉Dotnet Console
2. 更新 http://localhost:4200/, 應該回504, 伺服器無回應
3. 執行 Sun$ dotnet watch run
4. 測試兩個Button功能
## 版控
```shell
Dawn$ git add .
Dawn$ git commit -m "webapi2"
Dawn$ git tag webapi2
```
## 收尾
```shell
Dawn$ git checkout master
Dawn$ branch -d lab
```
## 完成品測試
建置專案
```shell
$ git clone https://github.com/hyflamewow/Dawn.git
Dawn$ git checkout -b lab webapi2
Moon$ npm i
Moon$ npm run build
Sun$ dotnet restore
Sun$ dotnet run
```
瀏覽  
http://localhost:5000/  
https://localhost:5001/

收尾
```shell
Dawn$ git checkout master
Dawn$ branch -d lab
```