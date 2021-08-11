import React, {useEffect, useState} from 'react';
import {Col, Row} from 'react-bootstrap';
import {makeStyles} from '@material-ui/core/styles';
import {useLocation} from 'react-router-dom';

import {BusinessDTO} from '../business/Business';
import {Card} from '../../common/components/card/Card';
import {generateTimeSlots} from '../timeslot/timeSlotSlice';
import {ComboBox, ComboBoxOption} from '../../common/components/form/ComboBox';
import DateUtil from '../../common/util/DateUtil';
import {ErrorView} from '../../common/views/ErrorView';
import {Header} from '../../common/components/Texts';
import {selectApiState} from '../../common/api/apiSlice';
import {useAppDispatch, useAppSelector} from '../../app/Store';

const useStyles = makeStyles((theme) => ({
    button: {
        marginTop: 100,
        width: '55%',
    },
    card: {
        borderRadius: 15,
        height: 400,
        textAlign: 'center',
    },
    row: {
        justifyContent: 'center',
    },
}));

interface LocationState {
    business: BusinessDTO;
}

export const GenerateTimeSlotsView: React.FC = () => {
    const styles = useStyles();
    const location = useLocation<LocationState>();

    const dispatch = useAppDispatch();
    const apiState = useAppSelector(selectApiState);

    const [dateOptions, setDateOptions] = useState<ComboBoxOption[]>(DateUtil.getNext7Days());
    const [selectedDate, setSelectedDate] = useState<ComboBoxOption>();

    if (!location.state) {
        return <ErrorView />;
    }

    const business = location.state.business;

    useEffect(() => {
        setDateOptions(dateOptions.filter((date) => date.label !== selectedDate?.label));
    }, [selectedDate]);

    return (
        <>
            <Row className={styles.row}>
                <Header text={business.name} />
            </Row>
            <Row className={styles.row}>
                <Col lg={6}>
                    {/*      <Modal
                        show={apiInfo.message ? true : false}
                        title={
                            apiInfo.message !== TIMESLOTS_GENERATED_MSG
                                ? apiInfo.message
                                : selectedDate &&
                                  `Time slots added on ${selectedDate.label.substring(
                                      selectedDate.label.indexOf(',') + 1
                                  )}`
                        }
                        text={apiInfo.message}
                        primaryAction={() => {
                            dispatch(clearTimeSlotsApiInfo());
                            history.push('/business/timeslots/manage', {business});
                        }}
                        primaryActionText="See time slots"
                        secondaryAction={() => dispatch(clearTimeSlotsApiInfo())}
                    /> */}
                    <Card
                        className={styles.card}
                        title="Generate Time Slots"
                        subtitle="This will generate time slots in opening hours on the selected date"
                        variant="outlined"
                        buttonText="Generate"
                        buttonColor="primary"
                        buttonStyle={styles.button}
                        buttonSize="large"
                        disableButton={!selectedDate || !dateOptions.length}
                        buttonAction={() =>
                            dispatch(
                                generateTimeSlots({
                                    businessId: business.id,
                                    start: selectedDate!.value!,
                                })
                            )
                        }
                    >
                        <ComboBox
                            style={{marginTop: 10, marginLeft: 110, width: '60%'}}
                            label="Pick a date"
                            defaultLabel="Time slots already generated"
                            id="email"
                            type="text"
                            options={dateOptions}
                            setFieldValue={(option: ComboBoxOption) => setSelectedDate(option)}
                        />
                    </Card>
                </Col>
            </Row>
        </>
    );
};
