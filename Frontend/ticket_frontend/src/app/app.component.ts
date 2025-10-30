import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';

interface AvailableBus {
  busScheduleId: string;
  busName: string;
  companyName: string;
  fromCity: string;
  toCity: string;
  startTime: string;
  arrivalTime: string;
  price: number;
  seatsLeft: number;
}

interface Seat {
  seatNumber: string;
  status: string;
}

interface SeatPlan {
  seats: Seat[];
}

interface BookingResult {
  success: boolean;
  message: string;
  ticketId?: string;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
 
  private apiUrl = 'http://localhost:5106/api';

  currentView: 'search' | 'seats' | 'success' = 'search';
  
  searchParams = {
    fromCity: '',
    toCity: '',
    journeyDate: ''
  };

  buses: AvailableBus[] = [];
  searchCompleted = false;
  selectedBus: AvailableBus | null = null;
  seatPlan: Seat[] = [];
  selectedSeats: string[] = [];
  isBooking = false;
  bookingMessage = '';
  ticketId = '';

  passengerInfo = {
    name: '',
    mobile: '',
    boardingPoint: '',
    droppingPoint: ''
  };

  constructor(private http: HttpClient) {}

  searchBuses(): void {
    this.searchCompleted = false;
    const params = {
      from: this.searchParams.fromCity,
      to: this.searchParams.toCity,
      journeyDate: this.searchParams.journeyDate
    };

    this.http.get<AvailableBus[]>(`${this.apiUrl}/search/available-buses`, { params })
      .subscribe({
        next: (data) => {
          this.buses = data;
          this.searchCompleted = true;
        },
        error: (error) => {
          console.error('Search error:', error);
          this.buses = [];
          this.searchCompleted = true;
          alert('Error searching buses. Please try again.');
        }
      });
  }

  viewSeats(bus: AvailableBus): void {
    this.selectedBus = bus;
    this.http.get<SeatPlan>(`${this.apiUrl}/booking/seat-plan/${bus.busScheduleId}`)
      .subscribe({
        next: (data) => {
          this.seatPlan = data.seats;
          this.currentView = 'seats';
          this.selectedSeats = [];
          this.passengerInfo = {
            name: '',
            mobile: '',
            boardingPoint: bus.fromCity,
            droppingPoint: bus.toCity
          };
        },
        error: (error) => {
          console.error('Error loading seats:', error);
          alert('Error loading seat plan. Please try again.');
        }
      });
  }

  toggleSeat(seat: Seat): void {
    if (seat.status !== 'Available') return;

    const index = this.selectedSeats.indexOf(seat.seatNumber);
    if (index > -1) {
      this.selectedSeats.splice(index, 1);
    } else {
      this.selectedSeats.push(seat.seatNumber);
    }
  }

  isSelected(seatNumber: string): boolean {
    return this.selectedSeats.includes(seatNumber);
  }

  calculateTotal(): number {
    return this.selectedSeats.length * (this.selectedBus?.price || 0);
  }

  confirmBooking(): void {
    if (this.selectedSeats.length === 0) {
      alert('Please select at least one seat');
      return;
    }

    this.isBooking = true;
    const bookingData = {
      busScheduleId: this.selectedBus?.busScheduleId,
      seatNumbers: this.selectedSeats,
      passengerName: this.passengerInfo.name,
      passengerMobile: this.passengerInfo.mobile,
      boardingPoint: this.passengerInfo.boardingPoint,
      droppingPoint: this.passengerInfo.droppingPoint
    };

    this.http.post<BookingResult>(`${this.apiUrl}/booking/book-seat`, bookingData)
      .subscribe({
        next: (result) => {
          this.isBooking = false;
          if (result.success) {
            this.bookingMessage = result.message;
            this.ticketId = result.ticketId || '';
            this.currentView = 'success';
          } else {
            alert(result.message);
          }
        },
        error: (error) => {
          this.isBooking = false;
          console.error('Booking error:', error);
          alert('Booking failed. Please try again.');
        }
      });
  }

  backToSearch(): void {
    this.currentView = 'search';
    this.selectedBus = null;
    this.selectedSeats = [];
    this.seatPlan = [];
  }
}