import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-clock',
  templateUrl: './clock.component.html'
})
export class ClockComponent {
  dateTimeData: DateTimeData;
  showTimezonesList: boolean;
  timezones: any[];
  time: number;
  error: boolean;
  errorMessage: string;
  isLoading: boolean;

  private intervalId;

  ngOnInit() {
    this.intervalId = setInterval(() => this.updateTime(), 1000);
  }

  ngOnDestroy() {
    clearInterval(this.intervalId);
  }

  updateTime() {
    var localUtcOffset = new Date().getTimezoneOffset() * 60 * 1000;
    this.time = new Date().getTime() + localUtcOffset + this.dateTimeData.utcOffset;
  }

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.isLoading = true;

    this.dateTimeData = <DateTimeData>{};
    http.get<DateTimeData>(baseUrl + 'api/dateTimeData').subscribe(result => {
      this.dateTimeData = result;
      this.updateTime();
      this.isLoading = false;
    }, error => {
        this.error = true;
        if (error.status == 500) {
          this.errorMessage = "Strefa czasowa nie jest prawidłowo skonfigurowana" +
          " lub podano nieistniejącą strefę czasową.";

          this.showTimezonesList = true;
          http.get<string[]>(baseUrl + 'api/dateTimeData/timezones').subscribe(result => {
            this.timezones = result;
            this.isLoading = false;
          });
        }
        else if (error.status == 502) {
          this.errorMessage = "Błąd: API niedostępne.";
          this.isLoading = false;
        }
        else {
          this.errorMessage = "Nieznany błąd: " + error.message;
          this.isLoading = false;
        }
    });
  }
}

interface DateTimeData {
  timezone: string;
  utcOffset: number;
}
