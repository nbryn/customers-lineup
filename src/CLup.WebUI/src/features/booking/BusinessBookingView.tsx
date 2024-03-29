import React from 'react';
import Chip from '@mui/material/Chip';
import {Col, Row} from 'react-bootstrap';
import makeStyles from '@mui/styles/makeStyles';

import {ErrorView} from '../../shared/views/ErrorView';
import {Header} from '../../shared/components/Texts';
import {selectCurrentBusiness} from '../business/BusinessState';
import type {TableColumn} from '../../shared/components/Table';
import {TableContainer} from '../../shared/containers/TableContainer';
import {useAppSelector} from '../../app/Store';
import {useDeleteBusinessBookingMutation} from './BookingApi';
import {type BookingDto} from '../../autogenerated';

const useStyles = makeStyles(() => ({
    row: {
        justifyContent: 'center',
    },
}));

export const BusinessBookingView: React.FC = () => {
    const styles = useStyles();
    const [deleteBookingForBusiness] = useDeleteBusinessBookingMutation();
    const business = useAppSelector(selectCurrentBusiness);

    if (!business) {
        return <ErrorView />;
    }

    const columns: TableColumn[] = [
        {title: 'id', field: 'id', hidden: true},
        {title: 'timeSlotId', field: 'timeSlotId', hidden: true},
        {title: 'User', field: 'userEmail'},
        {title: 'Interval', field: 'interval'},
        {title: 'Date', field: 'date'},
        {title: 'Capacity', field: 'capacity'},
    ];

    const actions = [
        {
            icon: () => <Chip size="small" label="Contact User" clickable color="primary" />,
            onClick: (_: React.ChangeEvent, rowData: BookingDto) => {
                console.log(rowData);
            },
        },
        {
            icon: () => <Chip size="small" label="Delete Booking" clickable color="secondary" />,
            onClick: async (_: React.ChangeEvent, rowData: BookingDto) => {
                await deleteBookingForBusiness({businessId: business.id!, bookingId: rowData.id!});
            },
        },
    ];

    return (
        <>
            <Row className={styles.row}>
                <Header text={`Bookings For ${business.name}`} />
            </Row>
            <Row className={styles.row}>
                <Col sm={6} md={8} lg={6} xl={10}>
                    <TableContainer
                        actions={actions}
                        columns={columns}
                        tableTitle="Bookings"
                        emptyMessage="No Bookings Yet"
                        tableData={business.bookings ?? []}
                    />
                </Col>
            </Row>
        </>
    );
};
