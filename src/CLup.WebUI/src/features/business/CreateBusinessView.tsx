import React from 'react';
import {Col, FormGroup, Row} from 'react-bootstrap';
import makeStyles from '@mui/styles/makeStyles';
import {MenuItem} from '@mui/material';

import {businessValidationSchema} from './BusinessValidation';
import {Card} from '../../shared/components/card/Card';
import {ComboBox, type ComboBoxOption} from '../../shared/components/form/ComboBox';
import {useCreateBusinessMutation} from './BusinessApi';
import {Form} from '../../shared/components/form/Form';
import {Header} from '../../shared/components/Texts';
import StringUtil from '../../shared/util/StringUtil';
import {TextField} from '../../shared/components/form/TextField';
import TextFieldUtil from '../../shared/util/TextFieldUtil';
import {useAddress} from '../../shared/hooks/useAddress';
import {type Index, useForm} from '../../shared/hooks/useForm';
import {BusinessType, type CreateBusinessRequest} from '../../autogenerated';

const useStyles = makeStyles(() => ({
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
    const [createBusiness] = useCreateBusinessMutation();

    const formValues = {
        id: '',
        name: '',
        zip: '',
        street: '',
        type: BusinessType.Bakery,
        capacity: 0,
        timeSlotLengthInMinutes: 0,
        opens: '',
        closes: '',
    };

    const {formHandler} = useForm<typeof formValues & Index>({
        initialValues: formValues,
        validationSchema: businessValidationSchema,
        onSubmit: async (formValues) => {
            const address = addressHandler.addresses.find(
                (address) => address.street === formValues.street
            );

            const createBusinessRequest = {
                ...formValues,
                address,
                businessHours: {
                    // TODO: This will not work if the user enters a time with minutes
                    start: {hour: parseInt(formValues.opens)},
                    end: {hour: parseInt(formValues.closes)},
                },
            } as CreateBusinessRequest;

            await createBusiness(createBusinessRequest);
        },
    });

    const addressHandler = useAddress(formHandler);
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
                            submitButtonText="Create"
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
                                                        select={key === 'type'}
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
                                                            Object.values(BusinessType).map(
                                                                (type) => (
                                                                    <MenuItem
                                                                        key={type}
                                                                        value={type}
                                                                    >
                                                                        {type}
                                                                    </MenuItem>
                                                                )
                                                            )}
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
