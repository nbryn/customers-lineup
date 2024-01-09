import React, {useEffect} from 'react';
import {Col, FormGroup, Row} from 'react-bootstrap';
import {makeStyles} from '@material-ui/core/styles';
import {MenuItem} from '@material-ui/core';

import {BusinessDTO} from './Business';
import {businessValidationSchema} from './BusinessValidation';
import {Card} from '../../shared/components/card/Card';
import {ComboBox, ComboBoxOption} from '../../shared/components/form/ComboBox';
import {createBusiness, fetchBusinessesTypes, selectBusinessTypes} from './BusinessState';
import {Form} from '../../shared/components/form/Form';
import {Header} from '../../shared/components/Texts';
import StringUtil from '../../shared/util/StringUtil';
import {TextField} from '../../shared/components/form/TextField';
import TextFieldUtil from '../../shared/util/TextFieldUtil';
import {useAddress} from '../../shared/hooks/useAddress';
import {useAppDispatch, useAppSelector} from '../../app/Store';
import {useForm} from '../../shared/hooks/useForm';

const useStyles = makeStyles((theme) => ({
    card: {
        marginTop: 20,
        borderRadius: 15,
        height: 600,
        textAlign: 'center',
    },
    formGroup: {
        marginBottom: 30,
    },
    helperText: {
        color: 'red',
    },
    textField: {
        width: '75%',
    },
    wrapper: {
        justifyContent: 'center',
    },
}));

export const CreateBusinessView: React.FC = () => {
    const styles = useStyles();
    const dispatch = useAppDispatch();
    const businessTypes = useAppSelector(selectBusinessTypes);

    const formValues: BusinessDTO = {
        id: '',
        name: '',
        zip: '',
        street: '',
        type: '',
        capacity: '',
        timeSlotLength: '',
        opens: '',
        closes: '',
    };

    const {formHandler} = useForm<BusinessDTO>({
        initialValues: formValues,
        validationSchema: businessValidationSchema,
        onSubmit: (business) => dispatch(createBusiness(business)),
        beforeSubmit: (business) => {
            const address = addressHandler.addresses.find((x) => x.street === business.street)!;
            const newBusiness = {...business};

            newBusiness.longitude = address.longitude;
            newBusiness.latitude = address.latitude;
            newBusiness.city = address.city;
            newBusiness.zip = address.zip;

            newBusiness.opens = business.opens.replace(':', '.');
            newBusiness.closes = business.closes.replace(':', '.');

            return newBusiness;
        },
    });

    const addressHandler = useAddress(formHandler);

    useEffect(() => {
        (async () => {
            dispatch(fetchBusinessesTypes());
        })();
    }, []);

    return (
        <>
            <Row className={styles.wrapper}>
                <Header text="New Business" />
            </Row>
            <Row className={styles.wrapper}>
                <Col sm={6} lg={8}>
                    <Card className={styles.card} title="Business Data" variant="outlined">
                        <Form
                            onSubmit={formHandler.handleSubmit}
                            buttonText="Create"
                            valid={formHandler.isValid}
                        >
                            <Row>
                                <Col sm={6} lg={6}>
                                    {Object.keys(formValues)
                                        .slice(1, 5)
                                        .map((key) => {
                                            if (key === 'zip' || key === 'street') {
                                                return (
                                                    <FormGroup
                                                        key={key}
                                                        className={styles.formGroup}
                                                    >
                                                        <ComboBox
                                                            id={key}
                                                            style={{
                                                                width: '75%',
                                                                marginLeft: 43,
                                                                marginTop: 25,
                                                            }}
                                                            label={StringUtil.capitalize(key)}
                                                            type="text"
                                                            options={addressHandler.getLabels(key)}
                                                            onBlur={formHandler.handleBlur}
                                                            setFieldValue={(
                                                                option: ComboBoxOption,
                                                                formFieldId
                                                            ) =>
                                                                formHandler.setFieldValue(
                                                                    formFieldId,
                                                                    option.label
                                                                )
                                                            }
                                                            error={
                                                                formHandler.touched[key] &&
                                                                Boolean(formHandler.errors[key])
                                                            }
                                                            helperText={
                                                                formHandler.touched[key] &&
                                                                formHandler.errors[key]
                                                            }
                                                            defaultLabel={
                                                                key === 'street'
                                                                    ? 'Street - After Zip'
                                                                    : ''
                                                            }
                                                        />
                                                    </FormGroup>
                                                );
                                            }
                                            return (
                                                <FormGroup key={key} className={styles.formGroup}>
                                                    <TextField
                                                        className={styles.textField}
                                                        id={key}
                                                        label={TextFieldUtil.mapKeyToLabel(key)}
                                                        type={TextFieldUtil.mapKeyToType(key)}
                                                        value={formHandler.values[key]}
                                                        onChange={formHandler.handleChange(key)}
                                                        onBlur={formHandler.handleBlur}
                                                        select={key === 'type' ? true : false}
                                                        error={
                                                            formHandler.touched[key] &&
                                                            Boolean(formHandler.errors[key])
                                                        }
                                                        helperText={
                                                            formHandler.touched[key] &&
                                                            formHandler.errors[key]
                                                        }
                                                    >
                                                        {key === 'type' &&
                                                            businessTypes.map((type) => (
                                                                <MenuItem key={type} value={type}>
                                                                    {type}
                                                                </MenuItem>
                                                            ))}
                                                    </TextField>
                                                </FormGroup>
                                            );
                                        })}
                                </Col>
                                <Col sm={6} lg={6}>
                                    {Object.keys(formValues)
                                        .slice(5)
                                        .map((key) => (
                                            <FormGroup key={key} className={styles.formGroup}>
                                                <TextField
                                                    className={styles.textField}
                                                    id={key}
                                                    label={TextFieldUtil.mapKeyToLabel(key)}
                                                    type={TextFieldUtil.mapKeyToType(key)}
                                                    value={formHandler.values[key]}
                                                    onChange={formHandler.handleChange}
                                                    onBlur={formHandler.handleBlur}
                                                    step={TextFieldUtil.mapKeyToStep(key)}
                                                    error={
                                                        formHandler.touched[key] &&
                                                        Boolean(formHandler.errors[key])
                                                    }
                                                    helperText={
                                                        formHandler.touched[key] &&
                                                        formHandler.errors[key]
                                                    }
                                                    inputLabelProps={{
                                                        shrink: TextFieldUtil.shouldInputLabelShrink(
                                                            key
                                                        ),
                                                    }}
                                                />
                                            </FormGroup>
                                        ))}
                                </Col>
                            </Row>
                        </Form>
                    </Card>
                </Col>
            </Row>
        </>
    );
};