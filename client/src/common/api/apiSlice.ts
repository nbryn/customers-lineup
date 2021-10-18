import {createSlice, isAnyOf} from '@reduxjs/toolkit';

import {
    createBooking,
    deleteBookingForBusiness,
    deleteBookingForUser,
    fetchBookingsByBusiness,
    fetchBookingsByUser,
} from '../../features/booking/bookingSlice';
import {
    createBusiness,
    fetchAllBusinesses,
    fetchBusinessesByOwner,
    fetchBusinessesTypes,
    updateBusinessInfo,
} from '../../features/business/businessSlice';
import {
    createEmployee,
    deleteEmployee,
    fetchEmployeesByBusiness,
} from '../../features/employee/employeeSlice';
import {fetchUserInsights} from '../../features/insights/insightsSlice';
import {
    deleteTimeSlot,
    fetchAvailableTimeSlotsByBusiness,
    fetchTimeSlotsByBusiness,
    generateTimeSlots,
} from '../../features/timeslot/timeSlotSlice';
import {
    login,
    register,
    fetchUserInfo,
    fetchUsersNotEmployedByBusiness,
    updateUserInfo,
} from '../../features/user/userSlice';
import {RootState} from '../../app/Store';

const BOOKING_CREATED_MSG = 'Success - Go to my bookings to see your bookings';
const BOOKING_DELETED_MSG = 'Booking Deleted';

const BUSINESS_CREATED_MSG = 'Business Created - Go to my businesses to see your businesses';
const BUSINESS_UPDATED_MSG = 'Business Updated';

const EMPLOYEE_CREATED_MSG = 'Employee Created - Go to my employees to see your employees';

const TIMESLOT_DELETED_MSG = 'Timeslot Deleted';
const TIMESLOTS_GENERATED_MSG = 'Success! Press see time slots to manage time slots.';

const USER_UPDATED_MSG = 'Info Updated.';

const LOGIN_FAILED_MSG = 'Wrong Email/Password';

export type ToastInfo = {
    buttonText: string;
    navigateTo: string;
};

interface ApiState {
    error: boolean;
    loading: boolean;
    message: string;
    toastInfo?: ToastInfo;
}

export const initialState: ApiState = {
    error: false,
    loading: false,
    message: '',
    toastInfo: undefined,
};

export const apiSlice = createSlice({
    name: 'api',
    initialState,
    reducers: {
        clearApiState: (state) => {
            state.loading = false;
            state.error = false;
            state.message = ''
            state.toastInfo = undefined;
        },
    },
    extraReducers: (builder) => {
        builder.addCase(createBooking.fulfilled, (state) => {
            state.loading = false;
            state.error = false;
            state.message = BOOKING_CREATED_MSG;
            state.toastInfo = {buttonText: 'My Bookings', navigateTo: '/user/bookings'};
        });

        builder.addCase(createBusiness.fulfilled, (state) => {
            state.loading = false;
            state.error = false;
            state.message = BUSINESS_CREATED_MSG;
            state.toastInfo = {buttonText: 'My Businesses', navigateTo: '/business'};
        });

        builder.addCase(createEmployee.fulfilled, (state) => {
            state.loading = false;
            state.error = false;
            state.message = EMPLOYEE_CREATED_MSG;
            state.toastInfo = {buttonText: 'My Employees', navigateTo: '/business/employees/manage'};
        });

        builder.addCase(updateBusinessInfo.fulfilled, (state) => {
            state.loading = false;
            state.error = false;
            state.message = BUSINESS_UPDATED_MSG;
        });

        builder.addCase(generateTimeSlots.fulfilled, (state) => {
            state.loading = false;
            state.error = false;
            state.message = TIMESLOTS_GENERATED_MSG;
            state.toastInfo = {buttonText: 'See time slots', navigateTo: '/business/timeslots/manage'};
        });

        builder.addCase(login.rejected, (state) => {
            state.loading = false;
            state.error = true;
            state.message = LOGIN_FAILED_MSG;
        });

        builder.addCase(deleteBookingForUser.fulfilled, (state) => {
            state.loading = false;
            state.error = false;
            state.message = BOOKING_DELETED_MSG;
        });

        builder.addCase(deleteTimeSlot.fulfilled, (state) => {
            state.loading = false;
            state.error = false;
            state.message = TIMESLOT_DELETED_MSG;
        });

        builder.addCase(updateUserInfo.fulfilled, (state) => {
            state.loading = false;
            state.error = false;
            state.message = USER_UPDATED_MSG;
        });

        builder.addMatcher(
            isAnyOf(
                deleteBookingForBusiness.fulfilled,
                fetchBookingsByUser.fulfilled,
                fetchBookingsByBusiness.fulfilled,
                fetchAllBusinesses.fulfilled,
                fetchBusinessesByOwner.fulfilled,
                fetchBusinessesTypes.fulfilled,
                deleteEmployee.fulfilled,
                fetchEmployeesByBusiness.fulfilled,
                fetchUserInsights.fulfilled,
                deleteTimeSlot.fulfilled,
                fetchAvailableTimeSlotsByBusiness.fulfilled,
                fetchTimeSlotsByBusiness.fulfilled,
                fetchUsersNotEmployedByBusiness.fulfilled,
                login.fulfilled,
                register.fulfilled,
                fetchUserInfo.fulfilled
            ),
            (state) => {
                state.loading = false;
                state.error = false;
                state.message = '';
            }
        );

        builder.addMatcher(
            isAnyOf(
                createBooking.pending,
                deleteBookingForBusiness.pending,
                deleteBookingForUser.pending,
                fetchBookingsByBusiness.pending,
                fetchBookingsByUser.pending,
                createBusiness.pending,
                fetchAllBusinesses.pending,
                fetchBusinessesByOwner.pending,
                fetchBusinessesTypes.pending,
                updateBusinessInfo.pending,
                createEmployee.pending,
                deleteEmployee.pending,
                fetchEmployeesByBusiness.pending,
                fetchUserInsights.pending,
                deleteTimeSlot.pending,
                fetchAvailableTimeSlotsByBusiness.pending,
                fetchTimeSlotsByBusiness.pending,
                generateTimeSlots.pending,
                fetchUsersNotEmployedByBusiness.pending,
                login.pending,
                register.pending,
                fetchUserInfo.pending,
                updateUserInfo.pending
            ),
            (state) => {
                state.loading = true;
            }
        );

        builder.addMatcher(
            isAnyOf(
                createBooking.rejected,
                deleteBookingForBusiness.rejected,
                deleteBookingForUser.rejected,
                fetchAllBusinesses.rejected,
                fetchBookingsByBusiness.rejected,
                fetchBookingsByUser.rejected,
                createBusiness.rejected,
                fetchAllBusinesses.rejected,
                fetchBusinessesByOwner.rejected,
                fetchBusinessesTypes.rejected,
                updateBusinessInfo.rejected,
                createEmployee.rejected,
                deleteEmployee.rejected,
                fetchEmployeesByBusiness.rejected,
                fetchUserInsights.rejected,
                deleteTimeSlot.rejected,
                fetchAvailableTimeSlotsByBusiness.rejected,
                fetchTimeSlotsByBusiness.rejected,
                generateTimeSlots.rejected,
                fetchUsersNotEmployedByBusiness.rejected,
                fetchUserInfo.rejected,
                register.rejected,
                updateUserInfo.rejected
            ),
            (state, action) => {
                state.loading = false;
                state.error = true;
                state.message = action.error.message!
            }
        );
    },
});

export const {clearApiState} = apiSlice.actions;

export const selectApiState = (state: RootState) => state.api;

export default apiSlice.reducer;