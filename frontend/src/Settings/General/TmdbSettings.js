import PropTypes from 'prop-types';
import React from 'react';
import FieldSet from 'Components/FieldSet';
import FormGroup from 'Components/Form/FormGroup';
import FormInputGroup from 'Components/Form/FormInputGroup';
import FormLabel from 'Components/Form/FormLabel';
import { inputTypes } from 'Helpers/Props';
import translate from 'Utilities/String/translate';

function TmdbSettings(props) {
  const {
    settings,
    onInputChange
  } = props;

  const {
    tmdbApiKey
  } = settings;

  return (
    <FieldSet legend={translate('TMDb')}>
      <FormGroup>
        <FormLabel>{translate('TMDbApiKey')}</FormLabel>

        <FormInputGroup
          type={inputTypes.PASSWORD}
          name="tmdbApiKey"
          helpText={translate('TMDbApiKeyHelpText')}
          onChange={onInputChange}
          {...tmdbApiKey}
        />
      </FormGroup>
    </FieldSet>
  );
}

TmdbSettings.propTypes = {
  settings: PropTypes.object.isRequired,
  onInputChange: PropTypes.func.isRequired
};

export default TmdbSettings;
